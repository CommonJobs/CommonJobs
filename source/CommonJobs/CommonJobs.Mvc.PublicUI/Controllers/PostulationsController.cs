using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Infrastructure.SharedLinks;
using CommonJobs.Raven.Mvc;
using CommonJobs.Raven.Infrastructure;
using NLog;

namespace CommonJobs.Mvc.PublicUI.Controllers
{
    public class PostulationsController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private TemporalFileReference SaveTemporalFile(HttpPostedFileBase file)
        {
            var temporalFolderPath = System.Configuration.ConfigurationManager.AppSettings["CommonJobs/TemporalUploadsPath"];
            if (!Directory.Exists(temporalFolderPath))
            {
                Directory.CreateDirectory(temporalFolderPath);
            }
            var internalFileName = Guid.NewGuid().ToString();
            var temporalFullName = Path.Combine(temporalFolderPath, internalFileName);
            file.SaveAs(temporalFullName);
            return new TemporalFileReference() 
            {
                OriginalFileName = Path.GetFileName(file.FileName),
                InternalFileName = internalFileName
            };
        }

        private void GenerateApplicant(Postulation postulation)
        {
            try
            {
                //TODO: move it to an Action
                var applicant = new Applicant()
                {
                    JobSearchId = postulation.JobSearchId,
                    Email = postulation.Email,
                    FirstName = postulation.FirstName,
                    LastName = postulation.LastName
                };
                RavenSession.Store(applicant);

                if (postulation.Curriculum != null)
                {
                    var curriculum = GenerateAttachment(applicant, postulation.Curriculum);
                    applicant.Notes = new List<ApplicantNote>() {
                    new ApplicantNote()
                    {
                        Note = "Curriculum",
                        NoteType = ApplicantNoteType.GeneralNote,
                        RealDate = DateTime.Now,
                        RegisterDate = DateTime.Now,
                        Attachment = curriculum
                    }};
                }
            }
            catch (Exception e)
            {
                log.ErrorException("Unexpected error generating applicant from postulation", e);
                //TODO: dump all postulation data
            }
        }

        private AttachmentReference GenerateAttachment(object entity, TemporalFileReference temporalReference)
        {
            AttachmentReference result;
            var temporalFolderPath = System.Configuration.ConfigurationManager.AppSettings["CommonJobs/TemporalUploadsPath"];
            var temporalFilePath = Path.Combine(temporalFolderPath, temporalReference.InternalFileName);
            using (var stream = System.IO.File.OpenRead(temporalFilePath))
            {
                result = ExecuteCommand(new SaveAttachment(
                    entity,
                    temporalReference.OriginalFileName,
                    stream));
            }
            System.IO.File.Delete(temporalFilePath);
            return result;
        }

        private void PrepareCreateView(JobSearch jobSearch)
        {
            var md = new MarkdownDeep.Markdown();
            ViewBag.JobSearchId = jobSearch.Id;
            ViewBag.Title = jobSearch.Title;
            ViewBag.PublicNotes = new MvcHtmlString(md.Transform(jobSearch.PublicNotes));
        }

        public ActionResult Create(long jobSearchNumber, string slug = null)
        {
            var jobSearch = RavenSession.Load<JobSearch>(jobSearchNumber);
            if (jobSearch == null || !jobSearch.IsPublic)
                return HttpNotFound();

            PrepareCreateView(jobSearch);
            return View();
        }

        [HttpPost]
        public ActionResult Create(Postulation postulation, HttpPostedFileBase curriculumFile)
        {
            var jobSearch = RavenSession.Load<JobSearch>(postulation.JobSearchId);
            if (jobSearch == null || !jobSearch.IsPublic)
                return HttpNotFound();

            try
            {
                if (curriculumFile != null)
                {
                    postulation.Curriculum = SaveTemporalFile(curriculumFile);
                }
            }
            catch (Exception e)
            {
                log.ErrorException("Error uploading file", e);
                ModelState.AddModelError("curriculumFile", "Error recibiendo el archivo, por favor intente nuevamente.");
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    GenerateApplicant(postulation);

                    return RedirectToAction("Thanks", "Postulations");
                }
                catch (Exception e)
                {
                    log.ErrorException("Unexpected error creating postulations", e);
                    ModelState.AddModelError(string.Empty, "Error inesperado, por favor intente más tarde.");
                }
            }

            PrepareCreateView(jobSearch);
            return View();
        }

        public ActionResult Thanks()
        {
            return View("Thanks");
        }
    }
}

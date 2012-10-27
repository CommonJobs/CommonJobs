using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;
using CommonJobs.Application.Indexes;
using CommonJobs.Domain;
using Raven.Client.Linq;
using CommonJobs.Application.AttachmentStorage;
using CommonJobs.Application.ApplicantSearching;
using CommonJobs.Mvc.UI.Infrastructure;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles="Users")]
    public class ApplicantsController : CommonJobsController
    {
        //
        // GET: /Applicants/
        public ViewResult Index(ApplicantSearchParameters searchParameters)
        {
            return View(searchParameters);
        }

        //
        // GET: /Employees/List?terms=Mar
        public JsonNetResult List(ApplicantSearchParameters searchParameters)
        {
            var query = new SearchApplicants(searchParameters);
            var results = Query(query);
            return Json(new
            {
                Items = results,
                Skipped = searchParameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        } 

        // GET: /Applicants/QuickSearchAutocomplete?terms=Mar
        public JsonResult QuickSearchAutocomplete(string term)
        {
            const int maxResults = 20;
            var list = RavenSession.Advanced.DatabaseCommands
                .GetTerms("Applicant/QuickSearch", "ByTerm", term, maxResults)
                .Where(x => x.StartsWith(term));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var newApplicant = new Applicant();
            RavenSession.Store(newApplicant);
            return RedirectToAction("Edit", new { id = newApplicant.Id });
        }

        [HttpPost]
        public ActionResult QuickAttachment()
        {
            Applicant applicant;
            //No me anda el binding normal
            var id = RouteData.Values["id"] as string;
            if (id == null)
            {
                applicant = new Applicant();
                RavenSession.Store(applicant);
            }
            else
            {
                applicant = RavenSession.Load<Applicant>(id);
                if (applicant == null)
                    return HttpNotFound();
            }

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                var attachments = attachmentReader
                    .Select(x => ExecuteCommand(new SaveAttachment(applicant, x.Key, x.Value)))
                    .ToArray();

                var notes = attachments.Select(x => new ApplicantNote() 
                    {
                        Attachment = x,
                        Note = "QuickAttachment!",
                        NoteType = ApplicantNoteType.GeneralNote,
                        RealDate = DateTime.Now,
                        RegisterDate = DateTime.Now
                    });

                if (applicant.Notes == null)
                {
                    applicant.Notes = notes.ToList();
                }
                else
                {
                    applicant.Notes.AddRange(notes);
                }

                return Json(new
                {
                    success = true,
                    entityId = applicant.Id,
                    editUrl = Url.Action("Edit", new { id = applicant.Id }),
                    attachment = attachments.FirstOrDefault(),
                    attachments = attachments
                });
            }
        }

        [SharedEntityAlternativeAuthorization]
        public ActionResult Edit(string id, string sharedCode = null) 
        {
            var applicant = RavenSession.Load<Applicant>(id);
            if (applicant == null)
                return HttpNotFound();
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    applicant = applicant,
                    forceReadOnly = sharedCode != null
                },
                500);
            return View();
        }

        [SharedEntityAlternativeAuthorization]
        public JsonNetResult Get(string id, string sharedCode = null) 
        {
            var applicant = RavenSession.Load<Applicant>(id);
            return Json(applicant);
        }

        public JsonNetResult Post(Applicant applicant)
        {
            RavenSession.Store(applicant);
            return Get(applicant.Id);
        }

        public ActionResult Delete(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            RavenSession.Delete(applicant);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SavePhoto(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                if (attachmentReader.Count != 1)
                    throw new NotSupportedException("One and only one photo is required.");

                var attachment = attachmentReader.First();

                applicant.Photo = ExecuteCommand(new SavePhotoAttachments(
                    applicant,
                    attachment.Key,
                    attachment.Value));
                return Json(new { success = true, attachment = applicant.Photo });
            }
        }
    }
}

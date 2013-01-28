using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Application.Indexes;
using CommonJobs.Domain;
using Raven.Client.Linq;
using CommonJobs.Application.AttachmentStorage;
using CommonJobs.Application.ApplicantSearching;
using CommonJobs.Mvc.UI.Infrastructure;
using Newtonsoft.Json.Linq;
using CommonJobs.Application.ApplicantFlow;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles="Users,ApplicantManagers")]
    [Documentation("manual-de-usuario/postulantes")]
    public class ApplicantsController : CommonJobsController
    {
        //
        // GET: /Applicants/
        public ViewResult Index(ApplicantSearchParameters searchParameters)
        {
            ViewBag.EventTypes = Query(new GetEventTypesQuery());
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

        [Documentation("manual-de-usuario/edicion-de-postulante")]
        public ActionResult Create(string name)
        {
            var newApplicant = new Applicant(name);
            RavenSession.Store(newApplicant);
            return RedirectToAction("Edit", new { id = newApplicant.Id });
        }

        [HttpPost]
        public ActionResult QuickAttachment()
        {
            Applicant applicant;
            //The normal binding does not work
            var id = RouteData.Values["id"] as string;
            var name = Request.Form["name"] as string;
            if (id == null)
            {
                applicant = new Applicant(name);
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

                var notes = attachments.Select(x => new NoteWithAttachment() 
                    {
                        Attachment = x,
                        Note = "QuickAttachment!",
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

        private JObject GetApplicantJsonWithModifiedDate(Applicant applicant)
        {
            var lastModified = RavenSession.Advanced.GetMetadataFor(applicant).Value<DateTime>("Last-Modified");
            var jsonApplicant = Newtonsoft.Json.Linq.JObject.FromObject(applicant);
            jsonApplicant["Last-Modified"] = lastModified;
            return jsonApplicant;
        }

        [SharedEntityAlternativeAuthorization]
        [Documentation("manual-de-usuario/edicion-de-postulante")]
        public ActionResult Edit(string id, string sharedCode = null) 
        {
            var applicant = RavenSession.Load<Applicant>(id);
            if (applicant == null)
                return HttpNotFound();
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    applicant = GetApplicantJsonWithModifiedDate(applicant),
                    forceReadOnly = sharedCode != null,
                    technicalSkillLevels = TechnicalSkillLevelExtensions.GetLevelNames()
                },
                500);
            return View();
        }

        [SharedEntityAlternativeAuthorization]
        public JsonNetResult Get(string id, string sharedCode = null) 
        {
            var applicant = RavenSession.Load<Applicant>(id);
            return Json(GetApplicantJsonWithModifiedDate(applicant));
        }

        public ActionResult Post(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            if (applicant == null)
                return HttpNotFound();
            this.TryUpdateModel(applicant);
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

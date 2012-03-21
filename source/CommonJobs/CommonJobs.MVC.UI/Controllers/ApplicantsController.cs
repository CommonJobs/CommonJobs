using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;
using CommonJobs.Mvc.UI.Models;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Domain;
using Raven.Client.Linq;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Infrastructure.ApplicantSearching;

namespace CommonJobs.Mvc.UI.Controllers
{
    [Authorize]
    public class ApplicantsController : CommonJobsController
    {
        //
        // GET: /Applicants/
        public ViewResult Index(ApplicantSearchParameters searchParameters)
        {
            return View(searchParameters);
        }

        public ViewResult List(ApplicantSearchParameters searchParameters)
        {
            var results = Query(new SearchApplicants(searchParameters));
            return View(results);
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

        public ActionResult Edit(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    applicant = applicant,
                    saveApplicantUrl = Url.Action("SaveApplicant"),
                    getApplicantUrl = Url.Action("GetApplicant"),
                    deleteApplicantUrl = Url.Action("DeleteApplicant")
                },
                500);
            return View(applicant);
        }

        public JsonNetResult GetApplicant(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            return Json(applicant);
        }

        public JsonNetResult SaveApplicant(Applicant applicant)
        {
            RavenSession.Store(applicant);
            return GetApplicant(applicant.Id);
        }

        public ActionResult DeleteApplicant(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            RavenSession.Delete(applicant);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SavePhoto(string id)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            var attachmentReader = new RequestAttachmentReader(Request);
            applicant.Photo = ExecuteCommand(new SavePhotoAttachments(
                applicant, 
                attachmentReader.FileName, 
                attachmentReader.Stream));
            return Json(new { success = true, attachment = applicant.Photo });
        }
    }
}

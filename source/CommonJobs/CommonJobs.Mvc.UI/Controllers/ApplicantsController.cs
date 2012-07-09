using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Domain;
using Raven.Client.Linq;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Infrastructure.ApplicantSearching;
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
                Skiped = searchParameters.Skip,
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

        [SharedEntityAlternativeAuthorization]
        public ActionResult Edit(string id, string sharedCode = null) 
        {
            var applicant = RavenSession.Load<Applicant>(id);
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
            var attachmentReader = new RequestAttachmentReader(Request);
            applicant.Photo = ExecuteCommand(new SavePhotoAttachments(
                applicant, 
                attachmentReader.FileName, 
                attachmentReader.Stream));
            return Json(new { success = true, attachment = applicant.Photo });
        }
    }
}

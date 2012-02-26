using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Mvc;
using CommonJobs.MVC.UI.Models;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Domain;
using Raven.Client.Linq;

namespace CommonJobs.MVC.UI.Controllers
{
    public class ApplicantsController : CommonJobsController
    {
        //
        // GET: /Applicants/

        public ViewResult Index(SearchModel searchModel)
        {
            return View(searchModel);
        }

        public ViewResult List(SearchModel searchModel)
        {
            var list = RavenSession
                .Query<Applicant_QuickSearch.Query, Applicant_QuickSearch>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.ByTerm.StartsWith(searchModel.Term))
                .As<Applicant>()
                //.AsProjection<EmployeeListView>() // EmployeeListView is an optimization, we do not need it yet
                .ToList();
            return View(list);
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
    }
}

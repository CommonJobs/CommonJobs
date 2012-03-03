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
    public class ApplicantsController : MvcBase.PersonController
    {
        //
        // GET: /Applicants/

        public ViewResult Index(ApplicantSearchModel searchModel)
        {
            return View(searchModel);
        }

        public ViewResult List(ApplicantSearchModel searchModel)
        {
            var query = RavenSession
                .Query<Applicant_QuickSearch.Query, Applicant_QuickSearch>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.ByTerm.StartsWith(searchModel.Term))
                .As<Applicant>()
                .AsEnumerable(); //TODO: add proper index in raven db and remove this line.

                //.AsProjection<EmployeeListView>() // EmployeeListView is an optimization, we do not need it yet

            if (searchModel.HaveInterview)
                query = query.Where(x => x.HaveInterview);

            if (searchModel.HaveTechnicalInterview)
                query = query.Where(x => x.HaveTechnicalInterview);

            if (searchModel.Highlighted)
                query = query.Where(x => x.IsHighlighted);

            var list = query.ToList();
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

        // GET: /Employees/Photo/applicants/2?fileName=thumb_foto1_medium-xdagbypk.jpg&contentType=image/jpeg
        // GET: /Employees/Photo/applicants/2?fileName=thumb_foto1_medium-xdagbypk.jpg
        // GET: /Employees/Photo/applicants/2
        // GET: /Employees/Photo/applicants/2?thumbnail=true
        [HttpGet]
        public ActionResult Photo(string id, bool thumbnail = false, string fileName = null, string contentType = "image/jpeg")
        {
            var employee = RavenSession.Load<Applicant>(id);
            return GetPersonPhoto(employee, thumbnail, fileName, contentType);
        }

        [HttpPost]
        public ActionResult Photo(string id, string fileName)
        {
            var employee = RavenSession.Load<Applicant>(id);
            return SavePhoto(employee, fileName, Request);
        }

        // GET: /Employees/Attachment/applicants/2?fileName=foto1_medium-xdagbypk.jpg&contentType=image/jpeg
        // GET: /Employees/Attachment/applicants/2?fileName=foto1_medium-xdagbypk.jpg
        // GET: /Employees/Attachment/applicants/2
        [HttpGet, ActionName("Attachment")]
        public ActionResult GetAttachment(string id, string fileName)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            return File(new AttachmentsHelper().ReadAttachment(applicant.Id, "Attachment", fileName), "application/octet-stream", fileName);
        }

        [HttpPost, ActionName("Attachment")]
        public ActionResult PostAttachment(string id, string fileName)
        {
            var applicant = RavenSession.Load<Applicant>(id);
            var attachmentHelper = new AttachmentsHelper();
            var attachment = attachmentHelper.SaveAttachment(applicant.Id, "Attachment", Request, fileName);
            return Json(new { success = true, attachment = attachment });
        }
    }
}

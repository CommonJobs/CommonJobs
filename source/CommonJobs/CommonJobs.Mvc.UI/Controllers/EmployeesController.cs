using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Indexes;
using Raven.Client.Linq;
using CommonJobs.Raven.Mvc;
using CommonJobs.Domain;
using CommonJobs.Utilities;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Infrastructure.EmployeeSearching;
using NLog;
using CommonJobs.Infrastructure.AttachmentSlots;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles="Users")]
    public class EmployeesController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        //
        // GET: /Employees/
        public ViewResult Index(EmployeeSearchParameters searchParameters)
        {
            AttachmentSlot[] slotsToShow = Query(new AttachmentSlotsQuery<Employee>());
            log.Dump(LogLevel.Debug, slotsToShow, "attachmentSlots");
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    attachmentSlots = slotsToShow
                },
                500);

            return View(searchParameters);
        }

        //
        // GET: /Employees/List?terms=Mar
        public JsonNetResult List(EmployeeSearchParameters searchParameters)
        {
            var query = new SearchEmployees(searchParameters);
            var results = Query(query);
            return Json(new
            {
                Items = results,
                Skiped = searchParameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        } 
        
        //
        // GET: /Employees/QuickSearchAutocomplete?terms=Mar
        public JsonResult QuickSearchAutocomplete(string term)
        {
            const int maxResults = 20;
            var list = RavenSession.Advanced.DatabaseCommands
                .GetTerms("Employee/QuickSearch", "ByTerm", term, maxResults)
                .Where(x => x.StartsWith(term));
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Create()
        {
            var newEmployee = new Employee();
            RavenSession.Store(newEmployee);
            return RedirectToAction("Edit", new { id = newEmployee.Id });  
        }

        [HttpPost]
        public ActionResult QuickAttachment()
        {
            //No me anda el binding normal
            var id = RouteData.Values["id"] as string;
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var employee = RavenSession.Load<Employee>(id);
            if (employee == null)
                return HttpNotFound();

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                var attachments = attachmentReader
                    .Select(x => ExecuteCommand(new SaveAttachment(employee, x.Key, x.Value)))
                    .ToArray();

                var notes = attachments.Select(x => new NoteWithAttachment()
                {
                    Attachment = x,
                    Note = "QuickAttachment!",
                    RealDate = DateTime.Now,
                    RegisterDate = DateTime.Now
                });

                if (employee.Notes == null)
                {
                    employee.Notes = notes.ToList();
                }
                else
                {
                    employee.Notes.AddRange(notes);
                }

                return Json(new
                {
                    success = true,
                    entityId = employee.Id,
                    editUrl = Url.Action("Edit", new { id = employee.Id }),
                    attachment = attachments.FirstOrDefault(),
                    attachments = attachments
                });
            }
        }
         
        public ActionResult Edit(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            if (employee == null)
                return HttpNotFound();

            //TODO: It is here only for demo purposes
            AttachmentSlot[] slotsToShow = Query(new AttachmentSlotsQuery<Employee>());
            log.Dump(LogLevel.Debug, slotsToShow);

            ScriptManager.RegisterGlobalJavascript(
                "ViewData", 
                new { 
                    employee = employee,
                    attachmentSlots = slotsToShow //TODO: It is here only for demo purposes
                }, 
                500);
            return View();
        }

        public JsonNetResult Get(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            return Json(employee);
        }   

        public JsonNetResult Post(Employee employee)
        {
            RavenSession.Store(employee);
            return Get(employee.Id);
        }

        public ActionResult Delete(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            RavenSession.Delete(employee);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SavePhoto(string id, string fileName)
        {
            var employee = RavenSession.Load<Employee>(id);

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                if (attachmentReader.Count != 1)
                    throw new NotSupportedException("One and only one photo is required.");

                var attachment = attachmentReader.First();

                employee.Photo = ExecuteCommand(new SavePhotoAttachments(
                    employee,
                    attachment.Key,
                    attachment.Value));
                return Json(new { success = true, attachment = employee.Photo });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Application.Indexes;
using Raven.Client.Linq;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Domain;
using CommonJobs.Utilities;
using CommonJobs.Application.AttachmentStorage;
using CommonJobs.Application.EmployeeSearching;
using NLog;
using CommonJobs.Application.AttachmentSlots;
using CommonJobs.Application;
using Raven.Abstractions.Data;
using CommonJobs.Mvc.UI.Infrastructure;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles="Users,EmployeeManagers")]
    [Documentation("docs/manual-de-usuario/empleados")]
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
                Skipped = searchParameters.Skip,
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

        public ActionResult Create(string name)
        {
            var newEmployee = CreateEmployee(name);
            return RedirectToAction("Edit", new { id = newEmployee.Id });  
        }

        private Employee CreateEmployee(string name)
        {
            var newEmployee = new Employee(name);
            RavenSession.Store(newEmployee);
            return newEmployee;
        }

        [HttpPost]
        public ActionResult QuickAttachment()
        {
            //The normal binding does not work
            var id = RouteData.Values["id"] as string;
            var slotId = Request.Form["slot"] as string;
            var name = Request.Form["name"] as string;
            var uploadToNotes = string.IsNullOrEmpty(slotId);

            Employee employee;
            if (id == null)
            {
                employee = CreateEmployee(name);
            }
            else
            {
                employee = RavenSession.Load<Employee>(id);
                if (employee == null)
                    return HttpNotFound();
            }

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                var reading = attachmentReader.Select(x => ExecuteCommand(new SaveAttachment(employee, x.Key, x.Value)));
                if (!uploadToNotes)
                {
                    reading = reading.Take(1);
                }
                var attachments = reading.ToArray();
                SlotWithAttachment added = null;

                if (string.IsNullOrEmpty(slotId))
                {
                    QuickAttachToNotes(employee, attachments);
                }
                else
                {
                    var slot = RavenSession.Load<AttachmentSlot>(slotId);
                    if (slot == null)
                        return HttpNotFound();

                    added = employee.AddAttachment(attachments.First(), slot);
                }

                return Json(new
                {
                    success = true,
                    entityId = employee.Id,
                    editUrl = Url.Action("Edit", new { id = employee.Id }),
                    attachment = attachments.FirstOrDefault(),
                    attachments = attachments,
                    added = added
                });
            }
        }

        private static void QuickAttachToNotes(Employee employee, AttachmentReference[] attachments)
        {
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
        }

        public ActionResult Edit(string id)
        {
            log.Info("Edit employee (id: " + id + ")");
            var employee = RavenSession.Load<Employee>(id);

            if (employee == null)
                return HttpNotFound();

            AttachmentSlot[] slotsToShow = Query(new AttachmentSlotsQuery<Employee>());

            ScriptManager.RegisterGlobalJavascript(
                "ViewData", 
                new { 
                    employee = employee,
                    attachmentSlots = slotsToShow,
                    technicalSkillLevels = TechnicalSkillLevelExtensions.GetLevelNames()
                }, 
                500);
            return View();
        }

        public JsonNetResult Get(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            return Json(employee);
        }

        public ActionResult Post(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            if (employee == null)
                return HttpNotFound();
            this.TryUpdateModel(employee);
            return Json(employee);
        }

        public ActionResult Delete(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            RavenSession.Delete(employee);
            if (employee == null)
                return HttpNotFound();
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

        [HttpPost]
        public ActionResult HireApplicant(string applicantId)
        {
            var applicant = RavenSession.Load<Applicant>(applicantId);
            if (applicant == null)
                return HttpNotFound();
            
            var employee = applicant.Hire(DateTime.Now);
            RavenSession.Store(employee);

            return Json(new 
            { 
                success = true, 
                applicantId = applicantId, 
                employeeId = employee.Id 
            });
        }
    }
}
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
            //TODO: only for demo purposes
            /* NOTE:
             * Esto no funcionaba:
             *      .Where(x => x.RelatedEntityType == typeof(Employee))
             * porque Newtonsoft Json serializa el tipo con el nombre largo ("CommonJobs.Domain.Employee, CommonJobs.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") 
             * y RavenDB busca por el corto (CommonJobs.Domain.Employee)
             * 
             * Otra opción sería buscar por prefijo del id (http://mattwarren.org/2012/07/12/fun-with-ravendb-documents-keys/)
             * */
            //TODO refactor it as Query
            var slotsToShow = RavenSession.Query<AttachmentSlot>().Where(x => x.RelatedEntityTypeName == typeof(Employee).Name).ToList();
            log.Dump(LogLevel.Debug, slotsToShow);

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
         
        public ActionResult Edit(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            if (employee == null)
                return HttpNotFound();
            ScriptManager.RegisterGlobalJavascript(
                "ViewData", 
                new { 
                    employee = employee
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
            var attachmentReader = new RequestAttachmentReader(Request);
            employee.Photo = ExecuteCommand(new SavePhotoAttachments(
                employee, 
                attachmentReader.FileName, 
                attachmentReader.Stream));
            return Json(new { success = true, attachment = employee.Photo });
        }
    }
}
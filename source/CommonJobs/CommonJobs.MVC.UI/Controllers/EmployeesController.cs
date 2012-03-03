using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.MVC.UI.Models;
using Raven.Client.Linq;
using CommonJobs.Mvc;
using CommonJobs.Domain;
using CommonJobs.Utilities;

namespace CommonJobs.MVC.UI.Controllers
{
    public class EmployeesController : MvcBase.PersonController
    {
        //
        // GET: /Employees/
        public ViewResult Index(SearchModel searchModel)
        {
            return View(searchModel);
        }

        //
        // GET: /Employees/List?terms=Mar
        public ViewResult List(SearchModel searchModel)
        {
            var list = RavenSession
                .Query<Employee_QuickSearch.Query, Employee_QuickSearch>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.ByTerm.StartsWith(searchModel.Term))
                .As<Employee>()
                //.AsProjection<EmployeeListView>() // EmployeeListView is an optimization, we do not need it yet
                .ToList();
            return View(list);
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
            ScriptManager.RegisterGlobalJavascript(
                "ViewData", 
                new { 
                    employee = employee,    
                    saveEmployeeUrl = Url.Action("SaveEmployee"),
                    getEmployeeUrl = Url.Action("GetEmployee")
                }, 
                500);
            return View(employee);
        }

        public JsonNetResult GetEmployee(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            return Json(employee);
        }   

        public JsonNetResult SaveEmployee(Employee employee)
        {
            RavenSession.Store(employee);
            return GetEmployee(employee.Id);
        }
         
        public ActionResult Delete(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            RavenSession.Delete(employee);
            return RedirectToAction("Index");
        }

        // GET: /Employees/Photo/employees/2?fileName=thumb_foto1_medium-xdagbypk.jpg&contentType=image/jpeg
        // GET: /Employees/Photo/employees/2?fileName=thumb_foto1_medium-xdagbypk.jpg
        // GET: /Employees/Photo/employees/2
        // GET: /Employees/Photo/employees/2?thumbnail=true
        [HttpGet]
        public ActionResult Photo(string id, bool thumbnail = false, string fileName = null, string contentType = "image/jpeg")
        {
            var employee = RavenSession.Load<Employee>(id);
            return GetPersonPhoto(employee, thumbnail, fileName, contentType);
        }

        [HttpPost]
        public ActionResult Photo(string id, string fileName)
        {
            var employee = RavenSession.Load<Employee>(id);
            return SavePhoto(employee, fileName, Request);
        }
    }
}
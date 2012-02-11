using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Indexes;
using EmployeeFile.Models;
using Raven.Client.Linq;
using CommonJobs.Mvc;
using CommonJobs.Domain;

namespace EmployeeFile.Controllers
{
    public class EmployeesController : CommonJobsController
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
        
        //
        // GET: /Employees/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Employees/Create

        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                RavenSession.Store(employee);
                return RedirectToAction("Index");  
            }

            return View(employee);
        }
        
        //
        // GET: /Employees/Edit/5
 
        public ActionResult Edit(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            ScriptManager.RegisterGlobalJavascript(
                "Model", 
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
            return Json(employee); //TODO: check result javascript
        }   

        public JsonNetResult SaveEmployee(Employee employee)
        {
            RavenSession.Store(employee);
            return GetEmployee(employee.Id);
        }

        //
        // GET: /Employees/Delete/5
 
        public ActionResult Delete(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            return View(employee);
        }

        //
        // POST: /Employees/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            RavenSession.Delete(employee);
            return RedirectToAction("Index");
        }
    }
}
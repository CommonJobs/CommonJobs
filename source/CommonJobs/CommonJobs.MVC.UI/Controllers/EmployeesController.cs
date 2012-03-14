﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Mvc.UI.Models;
using Raven.Client.Linq;
using CommonJobs.Raven.Mvc;
using CommonJobs.Domain;
using CommonJobs.Utilities;
using CommonJobs.Infrastructure.AttachmentStorage;

namespace CommonJobs.Mvc.UI.Controllers
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
                .OrderBy(x => x.SortingField)
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
                    getEmployeeUrl = Url.Action("GetEmployee"),
                    deleteEmployeeUrl = Url.Action("DeleteEmployee")
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

        public ActionResult DeleteEmployee(string id)
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
            employee.Photo = ExecuteCommand(new SavePhotoAttachments()
            {
                RelatedEntity = employee,
                FileName = attachmentReader.FileName,
                Stream = attachmentReader.Stream,
                UploadPath = CommonJobs.Mvc.UI.Properties.Settings.Default.UploadPath 
            });
            return Json(new { success = true, attachment = employee.Photo });
        }
    }
}
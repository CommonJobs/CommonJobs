using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmployeeFile.Infrastructure.Indexes;
using EmployeeFile.Models;
using Raven.Client.Linq;

namespace EmployeeFile.Controllers
{
    public class EmployeesController : RavenController
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
                .Where(x => x.ByTerms.StartsWith(searchModel.Terms))
                .AsProjection<EmployeeListView>()
                .ToList();
            return View(list);
        }

        //
        // GET: /Employees/Details/5

        public ViewResult Details(string id)
        {
            var employee = RavenSession.Load<Employee>(id);
            return View(employee);
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
            return View(employee);
        }

        //
        // POST: /Employees/Edit/5

        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                RavenSession.Store(employee);
                return RedirectToAction("Index");
            }
            return View(employee);
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
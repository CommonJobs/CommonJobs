using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Application;
using CommonJobs.Application.Vacations;
using CommonJobs.Infrastructure.Mvc;
using NLog;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles = "Users,EmployeesManagers")]
    public class VacationsController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public ActionResult Index(int yquantity = 6, int bsize = 20)
        {
            var currentYear = DateTime.Now.Year;
            var years = Enumerable.Range(currentYear + 1 - yquantity, yquantity).Reverse().ToArray();

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new {
                    currentYear = currentYear,
                    years = years,
                    bsize = bsize
                }, 500);

            ViewBag.years = years;

            return View();
        }

        public JsonNetResult VacationBunch(BaseSearchParameters parameters)
        {
            var query = new SearchVacations(parameters);
            var results = Query(query);
            return Json(new
            {
                Items = results,
                Skipped = parameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        }
    }
}

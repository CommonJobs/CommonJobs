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
    [CommonJobsAuthorize(Roles = "Users,EmployeeManagers")]
    public class AbsencesController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private IEnumerable<DateTime> GetDays(int year)
        {
            var date = new DateTime(year, 1, 1);
            while (date.Year == year)
            {
                yield return date;
                date = date.AddDays(1);
            }
        }

        public ActionResult Index(int year = 0, int bsize = 20)
        {
            var currentYear = DateTime.Now.Year;
            year = year > 0 ? year : currentYear;

            var months = GetDays(year).GroupBy(x => x.Month).ToDictionary(x => x.Key, x => x.Select(y => y.Day).ToArray());
            ViewBag.Months = months;

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new {
                    year = year,
                    months = months,
                    currentYear = currentYear,
                    bsize = bsize
                }, 500);

            return View();
        }

        public JsonNetResult AbsenceBunch(int year, BaseSearchParameters parameters)
        {
            //TODO: return abscence data
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

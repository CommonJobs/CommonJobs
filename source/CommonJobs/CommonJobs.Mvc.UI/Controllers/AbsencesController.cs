using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Application;
using CommonJobs.Infrastructure.Mvc;
using NLog;
using CommonJobs.Application.EmployeeAbsences;
using CommonJobs.Mvc.UI.Infrastructure;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles = "Users,EmployeeManagers")]
    [Documentation("docs/manual-de-usuario/ausencias")]
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

        public ActionResult Index(int year = 0, int bsize = 10)
        {
            var currentYear = DateTime.Now.Year;
            year = year > 0 ? year : currentYear;

            ViewBag.Year = year;

            var reasons = Query(new GetAbsenceReasons());

            ViewBag.Reasons = reasons;

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new {
                    year = year,
                    currentYear = currentYear,
                    bsize = bsize
                }, 500);

            return View();
        }

        public JsonNetResult AbsenceBunch(int year, BaseSearchParameters parameters)
        {
            //TODO: return abscence data
            var query = new SearchAbsences(parameters) { From = new DateTime(year, 1, 1), To = new DateTime(year, 12, 31) };
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

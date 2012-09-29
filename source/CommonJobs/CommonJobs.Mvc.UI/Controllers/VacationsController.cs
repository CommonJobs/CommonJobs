using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure;
using CommonJobs.Infrastructure.Vacations;
using CommonJobs.Raven.Mvc;
using NLog;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class VacationsController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public ActionResult Index(int yquantity = 6, int bsize = 20)
        {
            var years = Enumerable.Range(DateTime.Now.Year + 1 - yquantity, yquantity).Reverse().ToArray();

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new { 
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

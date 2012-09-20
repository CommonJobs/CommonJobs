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

        public ActionResult Index()
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    now = DateTime.Now
                },
                500);
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

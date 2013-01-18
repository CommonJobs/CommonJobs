using CommonJobs.Infrastructure.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.Profiles
{
    public class ProfilesController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            return View();
        }
    }
}

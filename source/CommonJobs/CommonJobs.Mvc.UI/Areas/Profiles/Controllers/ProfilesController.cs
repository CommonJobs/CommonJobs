using CommonJobs.Infrastructure.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.Profiles
{
    public class ProfilesController : CommonJobsController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    [Authorize]
    public class FilesController : CommonJobsController
    {
        public ActionResult Index(string searchTerm)
        {
            //TODO do search
            return View(searchTerm);
        }
    }
}

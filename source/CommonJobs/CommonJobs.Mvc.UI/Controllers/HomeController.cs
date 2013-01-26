using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Mvc.UI.Infrastructure;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc.UI.Controllers
{
    [Documentation("manual-de-usuario/introduccion")]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}

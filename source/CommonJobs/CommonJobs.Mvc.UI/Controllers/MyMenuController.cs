using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class MyMenuController : Controller
    {
        //El propio menu
        public ActionResult Index()
        {
            return View("MyMenu");
        }

        //definición de los menues
        public ActionResult Admin()
        {
            return View();
        }

        //el menu de otro
        public ActionResult Edit(string domainUser)
        {
            return View("MyMenu");
        } 
    }
}

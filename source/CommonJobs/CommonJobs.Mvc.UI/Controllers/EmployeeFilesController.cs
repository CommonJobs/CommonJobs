using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    //NOTE: "Employee Files" as a name refers to the file that is the source of record for employee data
    //It is NOT meant as an synonym to "attachment"
    public class EmployeeFilesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}

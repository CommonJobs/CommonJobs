using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackbonePOC1.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCategories()
        {
            return Json(
                new object[]
                {
                    new { code = "All", description = "Mixed" },
                    new { code = "Processor", description = "Processor" },
                    new { code = "GraphicCard", description = "Graphic Card" },
                    new { code = "Motherboard", description = "Motherboard" },
                    new { code = "Desktop", description = "Desktop" },
                    new { code = "Notebook", description = "Notebook" },
                    new { code = "Server", description = "Server" },
                    new { code = "Workstation", description = "Workstation" },
                }
                , JsonRequestBehavior.AllowGet);
        }
    }
}

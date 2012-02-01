using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BackbonePOC1.Areas.Rest.Controllers
{
    public class CategoriesController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Get()
        {
            return Json(
                new object[]
                            {
                                new { code = "All", description = "Mixed", searchResultsView = "List" },
                                new { code = "Processor", description = "Processor", searchResultsView = "Grid" },
                                new { code = "GraphicCard", description = "Graphic Card", searchResultsView = "Grid" },
                                new { code = "Motherboard", description = "Motherboard" },
                                new { code = "Desktop", description = "Desktop" },
                                new { code = "Notebook", description = "Notebook" },
                                new { code = "Server", description = "Server" },
                                new { code = "Workstation", description = "Workstation" },
                            },
                JsonRequestBehavior.AllowGet);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BackbonePOC1.Models;

namespace BackbonePOC1.Areas.Rest.Controllers
{
    public class CategoriesController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Get()
        {
            return Json(
                new[]
                {
                    new CategoryConfiguration { Code = "All", Description = "Mixed", ViewType = Models.ViewType.List },
                    new CategoryConfiguration { Code = "Processor", Description = "Processor", ViewType = Models.ViewType.Grid },
                    new CategoryConfiguration { Code = "GraphicCard", Description = "Graphic Card", ViewType = Models.ViewType.Grid },
                    new CategoryConfiguration { Code = "Motherboard", Description = "Motherboard" },
                    new CategoryConfiguration { Code = "Desktop", Description = "Desktop" },
                    new CategoryConfiguration { Code = "Notebook", Description = "Notebook" },
                    new CategoryConfiguration { Code = "Server", Description = "Server" },
                    new CategoryConfiguration { Code = "Workstation", Description = "Workstation" },
                },
                JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Post()
        {
            return Json(true);
        }
    }
}

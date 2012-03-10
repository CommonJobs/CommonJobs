using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Mvc;

namespace CommonJobs.MVC.UI.Controllers
{
    public class AttachmentsController : Controller
    {
        //TODO: permitir no usar los nombres de las acciones
        [HttpGet]
        public ActionResult Get(string id, string contentType = "application/octet-stream")
        {
            return File(new AttachmentsHelper().ReadAttachment(id), contentType);
        }

        [HttpPost]
        public ActionResult Post(string fileName)
        {
            var attachment = new AttachmentsHelper().SaveAttachment(this.Request);
            return Json(new { success = true, attachment = attachment });
        }
    }
}

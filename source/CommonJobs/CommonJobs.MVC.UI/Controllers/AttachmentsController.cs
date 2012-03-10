using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Mvc;
using CommonJobs.MVC.UI.Attachments;

namespace CommonJobs.MVC.UI.Controllers
{
    public class AttachmentsController : CommonJobsController
    {
        private AttachmentsHelper attachmentsHelper = null;
        protected AttachmentsHelper AttachmentsHelper
        {
            get { return attachmentsHelper ?? (attachmentsHelper = new AttachmentsHelper(RavenSession)); }
        }

        //TODO: permitir no usar los nombres de las acciones
        [HttpGet]
        public ActionResult Get(string id, string contentType = "application/octet-stream")
        {
            return File(AttachmentsHelper.ReadAttachment(id), contentType);
        }

        [HttpPost]
        public ActionResult Post(string fileName)
        {
            var attachment = AttachmentsHelper.SaveAttachment(this.Request);
            return Json(new { success = true, attachment = attachment });
        }
    }
}

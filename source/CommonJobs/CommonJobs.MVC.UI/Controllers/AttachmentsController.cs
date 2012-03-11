using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Mvc;
using CommonJobs.MVC.UI.Attachments;
using CommonJobs.Domain;

namespace CommonJobs.MVC.UI.Controllers
{
    public class AttachmentsController : CommonJobsController
    {
        //TODO: permitir no usar los nombres de las acciones
        [HttpGet]
        public ActionResult Get(string id, string fileName = null)
        {
            var attachment = RavenSession.Load<Attachment>(id);
            if (attachment == null)
                return HttpNotFound();

            var stream = Query(new ReadAttachment() { Attachment = attachment });
            if (stream == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(fileName))
                return File(stream, attachment.ContentType);
            else
                return File(stream, attachment.ContentType, fileName);
        }

        [HttpPost]
        public ActionResult Post(string fileName)
        {
            var attachment = ExecuteCommand(new SaveAttachment() { Request = this.Request });
            return Json(new { success = true, attachment = attachment });
        }
    }
}

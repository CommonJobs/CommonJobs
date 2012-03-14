using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;
using CommonJobs.Mvc.UI.Attachments;
using CommonJobs.Domain;

namespace CommonJobs.Mvc.UI.Controllers
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

            var stream = Query(new ReadAttachment() 
            { 
                Attachment = attachment, 
                UploadPath = CommonJobs.Mvc.UI.Properties.Settings.Default.UploadPath 
            });
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
            var attachmentReader = new RequestAttachmentReader(Request);
            var attachment = ExecuteCommand(new SaveAttachment()
            {
                FileName = attachmentReader.FileName,
                Stream = attachmentReader.Stream
            });
            return Json(new { success = true, attachment = attachment });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Mvc;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Domain;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class AttachmentsController : CommonJobsController
    {
        //TODO: permitir no usar los nombres de las acciones
        [HttpGet]
        public ActionResult Get(string id, bool returnName = true)
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

            if (returnName)
                return File(stream, attachment.ContentType, attachment.FileName);
            else
                return File(stream, attachment.ContentType);                
        }

        [HttpPost]
        public ActionResult Post(string id)
        {
            var entity = RavenSession.Load<object>(id);
            if (entity == null)
                return HttpNotFound("Specified entity does not exists");
            
            var attachmentReader = new RequestAttachmentReader(Request);
            var attachment = ExecuteCommand(new SaveAttachment()
            {
                UploadPath = CommonJobs.Mvc.UI.Properties.Settings.Default.UploadPath,
                RelatedEntity = entity,
                FileName = attachmentReader.FileName,
                Stream = attachmentReader.Stream
            });

            return Json(new { success = true, attachment = attachment });
        }
    }
}

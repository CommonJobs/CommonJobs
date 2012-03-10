using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Mvc;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Utilities;
using CommonJobs.MVC.UI.Attachments;

namespace CommonJobs.MVC.UI.MvcBase
{
    public abstract class PersonController : CommonJobsController
    {
        //TODO: No estoy seguro, creo que esta clase no debería existir.
        private AttachmentsHelper attachmentsHelper = null;
        protected AttachmentsHelper AttachmentsHelper
        {
            get { return attachmentsHelper ?? (attachmentsHelper = new AttachmentsHelper(RavenSession));  }
        }

        protected ActionResult SavePhoto(Person person, HttpRequestBase request)
        {
            var photo = AttachmentsHelper.SaveAttachment(request);
            var thumbnail = SaveThumbnailAttachment(photo);
            var imageAttachment = new ImageAttachment() { Original = photo, Thumbnail = thumbnail };
            return Json(new { success = true, attachment = imageAttachment });
        }

        private AttachmentReference SaveThumbnailAttachment(AttachmentReference photo)
        {
            var thumbnailFileName = "thumb_" + photo.FileName;

            var photoStream = AttachmentsHelper.ReadAttachment(photo.Id);
            photoStream.Position = 0; //Find a more elegant way to do it
            //TODO: generate thumbnail
            var thumbnailStream = photoStream;

            return AttachmentsHelper.SaveAttachment(thumbnailFileName, thumbnailStream);
        }
    }
}
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

        protected ActionResult SavePhoto(Person person, HttpRequestBase request)
        {
            var attachmentHelper = new AttachmentsHelper();
            var photo = attachmentHelper.SaveAttachment(request);
            var thumbnail = SaveThumbnailAttachment(photo);
            var imageAttachment = new ImageAttachment() { Original = photo, Thumbnail = thumbnail };
            return Json(new { success = true, attachment = imageAttachment });
        }

        private AttachmentReference SaveThumbnailAttachment(AttachmentReference photo)
        {
            var attachmentHelper = new AttachmentsHelper();
            var thumbnailFileName = "thumb_" + photo.FileName;

            var photoStream = attachmentHelper.ReadAttachment(photo.Id);
            photoStream.Position = 0; //Find a more elegant way to do it
            //TODO: generate thumbnail
            var thumbnailStream = photoStream;

            return attachmentHelper.SaveAttachment(thumbnailFileName, thumbnailStream);
        }
    }
}
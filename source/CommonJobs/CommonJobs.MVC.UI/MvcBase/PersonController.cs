using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Mvc;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Utilities;

namespace CommonJobs.MVC.UI.MvcBase
{
    public abstract class PersonController : CommonJobsController
    {
        //TODO: remove this class

        //protected ActionResult GetPersonPhoto(Person person, bool thumbnail, string id, string contentType)
        //{
        //    if (person != null)
        //    {
        //        if (string.IsNullOrEmpty(id) && person.Photo.Original != null)
        //        {
        //            var attachment = thumbnail ? person.Photo.Thumbnail ?? person.Photo.Original : person.Photo.Original;
        //            id = attachment.Id;
        //            contentType = attachment.ContentType;
        //        }
        //        if (!string.IsNullOrEmpty(id))
        //            return File(new AttachmentsHelper().ReadAttachment(id), contentType);
        //    }
        //    return this.HttpNotFound();
        //}

        protected ActionResult SavePhoto(Person person, HttpRequestBase request)
        {
            var attachmentHelper = new AttachmentsHelper();
            var photo = attachmentHelper.SaveAttachment(request);
            var thumbnail = SaveThumbnailAttachment(photo);
            var imageAttachment = new ImageAttachment() { Original = photo, Thumbnail = thumbnail };
            return Json(new { success = true, attachment = imageAttachment });
        }

        private Attachment SaveThumbnailAttachment(Attachment photo)
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
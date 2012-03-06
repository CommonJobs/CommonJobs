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
        //TODO: Creo que hay un lugar mejor que un controller base para poner todo esto de las fotos. Pero por ahora es lo mas cómodo, luego lo refactorizamos.
        const string PhotoAttachmentDivision = "photo";

        protected ActionResult GetPersonPhoto(Person person, bool thumbnail, string fileName, string contentType)
        {
            if (person != null)
            {
                if (string.IsNullOrEmpty(fileName) && person.Photo.Original != null)
                {
                    var attachment = thumbnail ? person.Photo.Thumbnail ?? person.Photo.Original : person.Photo.Original;
                    fileName = attachment.FileName;
                    contentType = attachment.ContentType;
                }
                if (!string.IsNullOrEmpty(fileName))
                    return File(new AttachmentsHelper().ReadAttachment(person.Id, PhotoAttachmentDivision, fileName), contentType);
            }
            return this.HttpNotFound();
        }

        protected ActionResult SavePhoto(Person person, string fileName, HttpRequestBase request)
        {
            var attachmentHelper = new AttachmentsHelper();
            var photo = attachmentHelper.SaveAttachment(person.Id, PhotoAttachmentDivision, request, fileName);
            var thumbnail = SaveThumbnailAttachment(person.Id, photo);
            var imageAttachment = new ImageAttachment() { Original = photo, Thumbnail = thumbnail };
            return Json(new { success = true, attachment = imageAttachment });
        }

        private Attachment SaveThumbnailAttachment(string entityId, Attachment photo)
        {
            var attachmentHelper = new AttachmentsHelper();
            var stream = attachmentHelper.ReadAttachment(entityId, PhotoAttachmentDivision, photo.FileName);
            var attachment = new Attachment()
            {
                ContentType = "image/jpeg",
                FileName = "thumb_" + photo.FileName,
                OriginalFileName = "thumb_" + photo.OriginalFileName
            };
            attachmentHelper.SaveAttachment(entityId, PhotoAttachmentDivision, attachment.FileName, stream);
            return attachment;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using System.IO;
using System.Security.Cryptography;
using CommonJobs.Mvc;

namespace CommonJobs.MVC.UI.Attachments
{
    public class SavePhotoAttachments : Command<ImageAttachment>
    {
        public HttpRequestBase Request { get; set; }

        public override ImageAttachment ExecuteWithResult()
        {
            var photo = ExecuteCommand(new SaveAttachment() { Request = Request });
            var thumbnail = SaveThumbnailAttachment(photo);
            return new ImageAttachment() { Original = photo, Thumbnail = thumbnail };
        }

        private AttachmentReference SaveThumbnailAttachment(AttachmentReference photo)
        {
            var thumbnailFileName = "thumb_" + photo.FileName;
            var photoStream = Query(new ReadAttachment() { Id = photo.Id });
            photoStream.Position = 0; //Find a more elegant way to do it
            //TODO: generate thumbnail
            var thumbnailStream = photoStream;
            return ExecuteCommand(new SaveAttachment() { FileName = thumbnailFileName, Stream = thumbnailStream });
        }
    }
}
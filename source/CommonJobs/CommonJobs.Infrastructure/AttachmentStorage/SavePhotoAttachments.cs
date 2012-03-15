using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using System.IO;
using System.Security.Cryptography;
using CommonJobs.Raven.Infrastructure;

namespace CommonJobs.Infrastructure.AttachmentStorage
{
    public class SavePhotoAttachments : Command<ImageAttachment>
    {
        public object RelatedEntity { get; set; } 
        public string FileName { get; set; }
        public Stream Stream { get; set; }
        public string UploadPath { get; set; }

        public SavePhotoAttachments()
        {
            UploadPath = CommonJobs.Infrastructure.Properties.Settings.Default.UploadPath; //Default value
        }

        public override ImageAttachment ExecuteWithResult()
        {
            var photo = ExecuteCommand(new SaveAttachment() 
            {
                UploadPath = UploadPath,
                RelatedEntity = RelatedEntity,
                FileName = FileName, 
                Stream = Stream 
            });
            var thumbnail = SaveThumbnailAttachment(photo);
            return new ImageAttachment() { Original = photo, Thumbnail = thumbnail };
        }

        private AttachmentReference SaveThumbnailAttachment(AttachmentReference photoReference)
        {
            var thumbnailFileName = "thumb_" + photoReference.FileName;

            var photo = RavenSession.Load<Attachment>(photoReference.Id);

            var photoStream = Query(new ReadAttachment() 
            {
                Attachment = photo, 
                UploadPath = UploadPath 
            });
            photoStream.Position = 0; //Find a more elegant way to do it
            //TODO: generate thumbnail
            var thumbnailStream = photoStream;
            return ExecuteCommand(new SaveAttachment() 
            {
                UploadPath = UploadPath,
                RelatedEntity = RelatedEntity,
                FileName = thumbnailFileName, 
                Stream = thumbnailStream 
            });
        }
    }
}
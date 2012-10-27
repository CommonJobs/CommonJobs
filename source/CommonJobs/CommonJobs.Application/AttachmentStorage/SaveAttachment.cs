using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using System.IO;
using System.Security.Cryptography;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Application.AttachmentIndexing;

namespace CommonJobs.Application.AttachmentStorage
{
    public class SaveAttachment : Command<AttachmentReference>
    {
        public object RelatedEntity { get; set; } 
        public string FileName { get; set; }
        public Stream Stream { get; set; }
        public string UploadPath { get; set; }
        public string ExistingId { get; set; }

        public SaveAttachment(object relatedEntity, string fileName, Stream stream)
        {
            UploadPath = CommonJobs.Application.Properties.Settings.Default.UploadPath; //Default value
            RelatedEntity = relatedEntity;
            FileName = fileName;
            Stream = stream;
        }

        public SaveAttachment(object relatedEntity, string fileName, Stream stream, string id): this(relatedEntity, fileName, stream)
        {
            ExistingId = id;
        }

        public override AttachmentReference ExecuteWithResult()
        {
            if (Stream.Length == 0)
                throw new ApplicationException("Empty files not allowed");

            var relatedEntityId = RavenSession.Advanced.GetDocumentId(RelatedEntity);
            if (relatedEntityId == null)
                throw new ApplicationException("Supplied related entity is not stored in database yet");

            var attachment = string.IsNullOrEmpty(ExistingId)
                ? new Attachment(relatedEntityId, FileName)
                : new Attachment(relatedEntityId, FileName, ExistingId);

            //La grabación y acceso al archivo también se le podría tirar a la capa de dominio, pero depende de la infrastructura (el sistema de archivos)
            var path = attachment.GetServerPath(UploadPath);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                Stream.Position = 0; //Find a more elegant way to do it
                Stream.CopyTo(fs);
                fs.Close();
            }

            RavenSession.Store(attachment);

            ExecuteCommand(new IndexAttachment(attachment)
            {
                UploadPath = UploadPath
            });

            return attachment.CreateReference();
        }
    }
}
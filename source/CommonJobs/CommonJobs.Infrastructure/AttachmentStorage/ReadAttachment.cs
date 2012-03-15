using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CommonJobs.Raven.Infrastructure;

namespace CommonJobs.Infrastructure.AttachmentStorage
{
    public class ReadAttachment : Query<Stream>
    {
        public Attachment Attachment { get; set; }
        public string UploadPath { get; set; }

        public ReadAttachment(Attachment attachment)
        {
            UploadPath = CommonJobs.Infrastructure.Properties.Settings.Default.UploadPath; //Default value
            Attachment = attachment;
        }
        
        public override Stream Execute()
        {
            var path = Attachment.GetServerPath(UploadPath);
            if (!File.Exists(path))
            {
                //TODO: es conveniente disparar una excepción?
                return null;
            }
            else
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }
    }
}
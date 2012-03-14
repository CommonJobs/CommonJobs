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
        
        //TODO: Duplicated code
        private string GetAttachmentPath(string id)
        {
            var folder = id.Substring(0, 2);
            var filename = id.Substring(2);
            return Path.Combine(UploadPath, folder, filename);
        }

        public override Stream Execute()
        {
            var path = GetAttachmentPath(Attachment.Id);
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
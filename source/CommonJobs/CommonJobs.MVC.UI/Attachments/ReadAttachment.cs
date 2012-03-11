using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using CommonJobs.Mvc;

namespace CommonJobs.MVC.UI.Attachments
{
    public class ReadAttachment : Query<Stream>
    {
        private Regex sha1regex = new Regex("^([0-9]|[A-F]){40}$", RegexOptions.IgnoreCase);

        public string Id { get; set; }
        //TODO: Duplicated code
        public string UploadPath { get; set; }

        public ReadAttachment()
        {
            //TODO: Duplicated code
            UploadPath = CommonJobs.MVC.UI.Properties.Settings.Default.UploadPath;
        }

        //TODO: Duplicated code
        private string GetAttachmentPath(string id)
        {
            var folder = id.Substring(0, 2);
            var filename = id.Substring(2);
            return Path.Combine(UploadPath, folder, filename);
        }

        public override Stream Execute()
        {
            if (Id == null || !sha1regex.IsMatch(Id))
                throw new ArgumentException(string.Format("'{0}' is not a valid SHA-1 hash", Id), "Id");
            var id = Id.ToLower();
            var attachment = RavenSession.Load<Attachment>(id);
            var path = GetAttachmentPath(id);
            if (attachment == null || !File.Exists(path))
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
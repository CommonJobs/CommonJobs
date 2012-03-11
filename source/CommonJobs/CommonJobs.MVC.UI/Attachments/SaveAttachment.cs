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
    public class SaveAttachment : Command<AttachmentReference>
    {
        public string FileName { get; set; }
        public Stream Stream { get; set; }
        public HttpRequestBase Request
        {
            set
            {
                FileName = value.Params[FileNameRequestParameter] as string
                    ?? value.Params["HTTP_X_FILE_NAME"] as string
                    ?? Path.GetRandomFileName();

                // To handle differences in FireFox/Chrome/Safari/Opera
                Stream = value.Files.Count > 0
                    ? value.Files[0].InputStream
                    : value.InputStream;
            }
        }
        public string FileNameRequestParameter { get; set; }
        //TODO: Duplicated code
        public string UploadPath { get; set; }

        public SaveAttachment()
        {
            FileNameRequestParameter = "fileName";
            //TODO: Duplicated code
            UploadPath = CommonJobs.MVC.UI.Properties.Settings.Default.UploadPath;
        }

        private string CalculateSha1(Stream stream)
        {
            stream.Position = 0; //Find a more elegant way to do it
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                string hash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));
                return hash.Replace("-", "").ToLower();
            }
        }

        //TODO: Duplicated code
        private string GetAttachmentPath(string id)
        {
            var folder = id.Substring(0, 2);
            var filename = id.Substring(2);
            return Path.Combine(UploadPath, folder, filename);
        }

        public override AttachmentReference ExecuteWithResult()
        {
            var id = CalculateSha1(Stream);
            //TODO: I am loading PlainContent field here and it is not necessary, maybe it is better to change it to a query with projection
            var attachment = RavenSession.Load<Attachment>(id);
            if (attachment == null)
            {
                var path = GetAttachmentPath(id);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    Stream.Position = 0; //Find a more elegant way to do it
                    Stream.CopyTo(fs);
                    fs.Close();
                }

                attachment = new Attachment()
                {
                    ContentType = GetContentTypeFromExtension(Path.GetExtension(FileName)),
                    Id = id
                };
                
                RavenSession.Store(attachment);
            }

            return new AttachmentReference()
            {
                FileName = FileName,
                Id = id
            };
        }

        private static string GetContentTypeFromExtension(string extension)
        {
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".pjpg":
                case ".pjpeg":
                    return "image/pjpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".svg":
                    return "image/svg+xml";
                case ".tif":
                case ".tiff":
                    return "image/tiff";
                case ".ico":
                    return "image/vnd.microsoft.icon";
                default:
                    return "application/octet-stream";
            }
        }

    }
}
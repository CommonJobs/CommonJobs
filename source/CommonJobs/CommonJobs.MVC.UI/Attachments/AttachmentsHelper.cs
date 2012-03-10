using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using CommonJobs.Utilities;
using System.Text.RegularExpressions;
using CommonJobs.Utilities;
using System.Security.Cryptography;
using CommonJobs.Domain;
using Raven.Client;

namespace CommonJobs.MVC.UI.Attachments
{
    //TODO: Refactorize it as commands and/or queries
    //TODO: Mejorar otras cosas, por ejemplo tal vez no sea necesario guardar el mimeType en la referecia si el archivo esta indexado en raven
    public class AttachmentsHelper
    {
        public AttachmentsHelper(IDocumentSession ravenSession)
        {
            RavenSession = ravenSession;
        }

        protected readonly IDocumentSession RavenSession;

        public const string FileNameRequestParameter = "fileName";
        private Regex sha1regex = new Regex("^([0-9]|[A-F]){40}$", RegexOptions.IgnoreCase);

        private string GetAttachmentPath(string id)
        {
            if (id == null || !sha1regex.IsMatch(id))
                throw new ArgumentException(string.Format("'{0}' is not a valid SHA-1 hash", id), "id");

            var folder = id.Substring(0, 2);
            var filename = id.Substring(2);

            return Path.Combine(
                Path.GetFullPath(CommonJobs.MVC.UI.Properties.Settings.Default.UploadPath),
                folder, 
                filename);
        }

        public AttachmentReference SaveAttachment(HttpRequestBase request)
        {
            var fileName = 
                request.Params[FileNameRequestParameter] as string 
                ?? request.Params["HTTP_X_FILE_NAME"] as string 
                ?? Path.GetRandomFileName();
            /// <summary>
            /// To handle differences in FireFox/Chrome/Safari/Opera
            /// </summary>
            int contentLength;
            string contentType;
            Stream stream;
            if (request.Files.Count > 0)
            {
                var file = request.Files[0];
                contentLength = file.ContentLength;
                contentType = file.ContentType;
                stream = file.InputStream;
            }
            else
            {
                contentLength = request.ContentLength;
                contentType = request.ContentType;
                stream = request.InputStream;
            }
            return SaveAttachment(fileName, stream);
        }

        public AttachmentReference SaveAttachment(string fileName, Stream stream)
        {
            var id = CalculateSha1(stream);
            //TODO: I am loading PlainContent field here and it is not necessary, maybe it is better to change it to a query with projection
            var attachment = RavenSession.Load<Attachment>(id);
            if (attachment == null)
            {
                var path = GetAttachmentPath(id);
                var directory = Path.GetDirectoryName(path);
                Directory.CreateDirectory(directory);

                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.Position = 0; //Find a more elegant way to do it
                    stream.CopyTo(fs);
                    fs.Close();
                }

                attachment = CreateAttachment(id, fileName, stream);
            }

            var reference = new AttachmentReference()
            {
                ContentType = attachment.ContentType,
                FileName = fileName,
                Id = id
            };

            return reference;
        }

        //TODO: remove filename paramenter
        private Attachment CreateAttachment(string id, string fileName, Stream stream)
        {
            var attachment = new Attachment()
            {
                ContentType = DetectMimeType(fileName, stream),
                Id = id
            };

            //TODO: extract content

            RavenSession.Store(attachment);
            return attachment;
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

        //TODO: remove filename paramenter
        private string DetectMimeType(string fileName, Stream stream)
        {
            stream.Position = 0; //Find a more elegant way to do it
            //TODO: detect mime from content
            var extension = Path.GetExtension(fileName);
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

        public Stream ReadAttachment(string id)
        {
            id = id.ToLower();
            var attachment = RavenSession.Load<Attachment>(id);
            var path = GetAttachmentPath(id);
            if (attachment == null || !File.Exists(path))
            {
                return null;
            }
            else
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
        }

        //public Stream ReadAttachment(string resourceId, string division, Attachment attachment)
        //{
        //    return ReadAttachment(resourceId, division, attachment.FileName);
        //}
    }
}

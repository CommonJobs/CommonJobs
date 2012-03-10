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

namespace CommonJobs.Mvc
{
    public class AttachmentsHelper
    {
        public const string FileNameRequestParameter = "fileName";
        private Regex sha1regex = new Regex("^([0-9]|[A-F]){40}$", RegexOptions.IgnoreCase);

        private string GetAttachmentPath(string id)
        {
            if (id == null || !sha1regex.IsMatch(id))
                throw new ArgumentException(string.Format("'{0}' is not a valid SHA-1 hash", id), "id");

            id = id.ToLower();
            var folder = id.Substring(0, 2);
            var filename = id.Substring(2);

            return Path.Combine(
                Path.GetFullPath(CommonJobs.Mvc.Properties.Settings.Default.UploadPath),
                folder, 
                filename);
        }

        public Attachment SaveAttachment(HttpRequestBase request)
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

        public Attachment SaveAttachment(string fileName, Stream stream)
        {
            var id = CalculateSha1(stream);
            var mimeType = DetectMimeType(fileName, stream);

            var attachment = new Attachment()
            {
                //It could be ussefull: ContentLength = inputFile.ContentLength,
                ContentType = mimeType,
                FileName = fileName,
                Id = id
            };

            var path = GetAttachmentPath(id);
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory);
            if (!File.Exists(path))
            {
                using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    stream.Position = 0; //Find a more elegant way to do it
                    stream.CopyTo(fs);
                    fs.Close();
                }
            }
            return attachment;
        }

        private string CalculateSha1(Stream stream)
        {
            stream.Position = 0; //Find a more elegant way to do it
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                string hash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));
                return hash.Replace("-", "");
            }
        }

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
            var path = GetAttachmentPath(id);
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        //public Stream ReadAttachment(string resourceId, string division, Attachment attachment)
        //{
        //    return ReadAttachment(resourceId, division, attachment.FileName);
        //}
    }
}

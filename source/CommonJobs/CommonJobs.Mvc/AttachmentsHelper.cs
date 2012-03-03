using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using CommonJobs.Utilities;
using System.Text.RegularExpressions;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc
{
    public class AttachmentsHelper
    {
        private string CreateUniqueFileName(string originalFileName = null)
        {
            if (originalFileName == null)
            {
                return Path.GetRandomFileName();
            }
            else
            {
                var originalWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                var originalExtension = Path.GetExtension(originalFileName);
                var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                return Slug(string.Format("{0}-{1}{2}", originalWithoutExtension, random, originalExtension));
            }
        }

        /// <summary>
        /// To handle differences in FireFox/Chrome/Safari/Opera
        /// </summary>
        private class InputFileDescriptor
        {
            public string ContentType { get; set; }
            public int ContentLength { get; set; }
            public Stream InputStream { get; set; }

            public InputFileDescriptor(HttpRequestBase request)
            {
                if (request.Files.Count > 0)
                {
                    var file = request.Files[0];
                    ContentLength = file.ContentLength;
                    ContentType = file.ContentType;
                    InputStream = file.InputStream;
                }
                else
                {
                    ContentLength = request.ContentLength;
                    ContentType = request.ContentType;
                    InputStream = request.InputStream;
                }
            }
        }

        private string GetAttachmentPath(string resourceId, string division, string fileName)
        {
            //TODO: verificar que no sea posible escalar directirios, hacer tests:
            //resourceId no modifique el path si tiene alguna ruta absoluta o ".." por ejemplo
            //division no modifique el path si tiene "/" o ".."
            //fileName no modifique el path si tiene "/" o ".."            
            var sections = new List<string>() { Path.GetFullPath(CommonJobs.Mvc.Properties.Settings.Default.UploadPath) };
            //Maybe plain is better sections.AddRange(resourceId.Split(new[] { '/' }));
            sections.Add(Slug(resourceId));
            sections.Add(Slug(division));
            sections.Add(Slug(fileName));
            return Path.Combine(sections.ToArray());
        }

        private string Slug(string text)
        {
            //TODO: Testear que si entra algo ya sluggedo siempre salga lo mismo
            string str = text.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9-\.]", "-");
            str = Regex.Replace(str, @"[-]+", "-").Trim();
            str = Regex.Replace(str, @"[\.]+", ".").Trim();
            return text;
        }

        public Attachment SaveAttachment(string resourceId, string division, HttpRequestBase request, string originalFileName)
        {
            var inputFile = new InputFileDescriptor(request);
            var attachment = new Attachment()
            {
                //It could be ussefull: ContentLength = inputFile.ContentLength,
                ContentType = DetectMimeType(inputFile),
                OriginalFileName = originalFileName,
                FileName = CreateUniqueFileName(originalFileName),
            };
            SaveAttachment(resourceId, division, attachment.FileName, inputFile.InputStream);
            return attachment;
        }

        public void SaveAttachment(string resourceId, string division, string fileName, Stream stream)
        {
            var path = GetAttachmentPath(resourceId, division, fileName);
            var directory = Path.GetDirectoryName(path);
            System.IO.Directory.CreateDirectory(directory);
            using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                stream.CopyTo(fs);
                fs.Close();
            }
        }

        private string DetectMimeType(InputFileDescriptor inputFile)
        {
            //TODO: detect mime from content or extension
            return "image/jpeg";
        }

        public Stream ReadAttachment(string resourceId, string division, string fileName)
        {
            var path = GetAttachmentPath(resourceId, division, fileName);
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        //public Stream ReadAttachment(string resourceId, string division, Attachment attachment)
        //{
        //    return ReadAttachment(resourceId, division, attachment.FileName);
        //}
    }
}

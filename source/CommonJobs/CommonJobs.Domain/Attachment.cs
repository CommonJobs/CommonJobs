using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CommonJobs.Domain
{
    public class Attachment
    {
        public string Id { get; set; }
        public string RelatedEntityId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string ContentExtractorConfigurationHash { get; set; }
        public string PlainContent { get; set; }

        public Attachment()
        {
            //TODO: verify if it empty constructor is really necessary for RavenDB
        }

        public Attachment(string relatedEntityId, string filename)
        {
            RelatedEntityId = relatedEntityId;
            Id = string.Format("{0}/{1}", RelatedEntityId, Guid.NewGuid().ToString());
            FileName = filename;
            ContentType = GetContentTypeFromExtension(Path.GetExtension(FileName));
        }

        public string GetServerPath(string baseFolder)
        {
            var sections = new[] { baseFolder }.Union(Id.Split(new[] { '/' })).ToArray();
            return Path.Combine(sections);
        }

        public static string GetContentTypeFromExtension(string extension)
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

        public AttachmentReference CreateReference()
        {
            return new AttachmentReference()
            {
                FileName = FileName,
                Id = Id
            };
        }
    }
}

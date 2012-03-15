using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.ContentExtraction;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.AttachmentStorage;

namespace CommonJobs.Infrastructure.AttachmentIndexing
{
    public class IndexAttachment : Command
    {
        public ContentExtractionConfiguration Configuration { get; set; }
        public Attachment Attachment { get; set; }
        public string UploadPath { get; set; }

        public IndexAttachment()
        {
            Configuration = ContentExtractionConfiguration.Current; //Default value
        }

        public override void Execute()
        {
            var stream = Query(new ReadAttachment() 
            { 
                Attachment = Attachment,
                UploadPath = UploadPath
            });
            ExtractionResult result = null;
            foreach (var extractor in Configuration)
            {
                if (extractor.TryExtract(Attachment.FileName, stream, out result))
                    break;
            }
            if (result == null)
            {
                Attachment.PlainContent = null;
            }
            else
            {
                Attachment.ContentType = result.ContentType;
                Attachment.PlainContent = result.PlainContent;
            }
            Attachment.ContentExtractorConfigurationHash = Configuration.HashCode;
        }
    }
}

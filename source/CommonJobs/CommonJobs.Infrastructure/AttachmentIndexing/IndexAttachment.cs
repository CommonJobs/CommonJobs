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

        public IndexAttachment(Attachment attachment)
        {
            Configuration = ContentExtractionConfiguration.Current; //Default value
            UploadPath = CommonJobs.Infrastructure.Properties.Settings.Default.UploadPath; //Default value
            Attachment = attachment;
        }

        public override void Execute()
        {
            var stream = Query(new ReadAttachment(Attachment) 
            { 
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

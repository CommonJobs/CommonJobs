using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Infrastructure.RavenDb;
using CommonJobs.ContentExtraction;
using CommonJobs.Domain;
using CommonJobs.Application.AttachmentStorage;

namespace CommonJobs.Application.AttachmentIndexing
{
    public class IndexAttachment : Command
    {
        public ContentExtractionConfiguration Configuration { get; set; }
        public Attachment Attachment { get; set; }
        public string UploadPath { get; set; }

        public IndexAttachment(Attachment attachment)
        {
            Configuration = ContentExtractionConfiguration.Current; //Default value
            UploadPath = CommonJobs.Application.Properties.Settings.Default.UploadPath; //Default value
            Attachment = attachment;
        }

        public override void Execute()
        {
            var stream = Query(new ReadAttachment(Attachment) 
            { 
                UploadPath = UploadPath
            });
            ExtractionResult result = null;
            if (Configuration.TryExtract(Attachment.GetServerPath(UploadPath), stream, Attachment.FileName, out result))
            {
                Attachment.PlainContent = result.PlainContent;
                if (result.ContentType != null)
                    Attachment.ContentType = result.ContentType;
            }
            else
            {
                Attachment.PlainContent = null;
            }
            Attachment.ContentExtractorConfigurationHash = Configuration.HashCode;
        }
    }
}

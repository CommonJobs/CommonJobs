using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Domain;
using CommonJobs.ContentExtraction;
using CommonJobs.Infrastructure.Indexes;

namespace CommonJobs.Infrastructure.AttachmentIndexing
{
    public class GetNotIndexedAttachments : Query<Attachment[]>
    {
        public ContentExtractionConfiguration Configuration { get; set; }
        public int Quantity { get; set; }

        public GetNotIndexedAttachments(int quantity)
        {
            Configuration = ContentExtractionConfiguration.Current; //Default value
            Quantity = quantity;
        }

        public override Attachment[] Execute()
        {
            //Primero busco los que nunca se indexaron
            var neverIndexed = RavenSession.Query<Attachment, Attachments_ByContentExtractorConfigurationHash>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.ContentExtractorConfigurationHash == null)
                .Take(Quantity)
                .ToList();

            var hash = Configuration.HashCode;

            //Ahora busco los obsoletos
            var obsoleteIndex = neverIndexed.Count < Quantity
                ? RavenSession.Query<Attachment, Attachments_ByContentExtractorConfigurationHash>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .Where(x => x.ContentExtractorConfigurationHash != hash)
                    .Take(Quantity - neverIndexed.Count)
                    .AsEnumerable()
                : Enumerable.Empty<Attachment>();

            return neverIndexed.Union(obsoleteIndex).ToArray();
        }
    }
}

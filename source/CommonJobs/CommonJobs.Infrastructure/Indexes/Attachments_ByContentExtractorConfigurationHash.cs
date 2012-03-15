using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;

namespace CommonJobs.Infrastructure.Indexes
{
    public class Attachments_ByContentExtractorConfigurationHash : AbstractIndexCreationTask<Attachment>
    {
        public Attachments_ByContentExtractorConfigurationHash()
        {
            Map = attachments => from attachment in attachments
                                 select new { ContentExtractorConfigurationHash = attachment.ContentExtractorConfigurationHash };
        }
    }
}

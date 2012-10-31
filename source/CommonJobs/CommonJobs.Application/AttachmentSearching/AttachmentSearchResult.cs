using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;

namespace CommonJobs.Application.AttachmentSearching
{
    public class AttachmentSearchResult
    {
        public string AttachmentId { get; set; }
        public string ContentType  { get; set; }
        public string FileName { get; set; }
        public Person RelatedEntity { get; set; }
        public bool IsOrphan { get; set; }
        public string PartialText { get; set; }
    }
}

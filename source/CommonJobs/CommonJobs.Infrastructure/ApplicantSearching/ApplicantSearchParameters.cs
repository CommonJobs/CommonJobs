using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Infrastructure.ApplicantSearching
{
    public class ApplicantSearchParameters : BaseSearchParameters
    {
        public bool Highlighted { get; set; }
        public bool HaveInterview { get; set; }
        public bool HaveTechnicalInterview { get; set; }
        public bool SearchInAttachments { get; set; }
    }
}
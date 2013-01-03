using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Application.ApplicantSearching
{
    public class ApplicantSearchParameters : BaseSearchParameters
    {
        public bool Highlighted { get; set; }
        [Obsolete("Use WithEvents=entrevista-rrhh")]
        public bool HaveInterview { get; set; }
        [Obsolete("Use WithEvents=entrevista-tecnica")]
        public bool HaveTechnicalInterview { get; set; }
        public bool SearchInAttachments { get; set; }
        public string[] WithEvents { get; set; }
    }
}
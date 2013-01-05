using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Application.ApplicantSearching
{
    public class ApplicantSearchParameters : BaseSearchParameters
    {
        public bool Highlighted { get; set; }
        public bool SearchInAttachments { get; set; }
        public string[] WithEvents { get; set; }
    }
}
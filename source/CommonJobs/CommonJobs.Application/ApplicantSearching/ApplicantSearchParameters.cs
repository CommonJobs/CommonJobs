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
        public ApplicantHiredFilter Hired { get; set; }
    }

    public enum ApplicantHiredFilter
    {
        Exclude = 0,
        Include = 1,
        OnlyHired = 2
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.JobSearchSearching
{
    public class JobSearchSearchResult
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PublicNote { get; set; }
        public string PrivateNote { get; set; }
        public bool IsPublic { get; set; }
    }
}

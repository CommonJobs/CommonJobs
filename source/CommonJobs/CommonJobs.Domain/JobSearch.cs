using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class JobSearch
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PublicNotes { get; set; }
        public string PrivateNotes { get; set; }
        public bool IsPublic { get; set; }

        public string PublicCode { get; set; }
    }
}

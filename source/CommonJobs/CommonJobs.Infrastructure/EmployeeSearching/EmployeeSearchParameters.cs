using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Infrastructure.EmployeeSearching
{
    public class EmployeeSearchParameters
    {
        public string Term { get; set; }
        public bool SearchInNotes { get; set; }
        public bool SearchInAttachments { get; set; }
    }
}
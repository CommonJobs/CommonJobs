using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Application.EmployeeSearching
{
    public class EmployeeSearchParameters : BaseSearchParameters
    {
        public bool SearchInNotes { get; set; }
        public bool SearchInAttachments { get; set; }
    }
}
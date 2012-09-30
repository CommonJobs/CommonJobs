using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Infrastructure.EmployeeSearching
{
    public class EmployeeSearchParameters : BaseSearchParameters
    {
        public EmployeeSearchParameters() { }
        public EmployeeSearchParameters(BaseSearchParameters baseParameters)
        {
            this.Term = baseParameters.Term;
            this.Skip = baseParameters.Skip;
            this.Take = baseParameters.Take;
        }

        public bool SearchInNotes { get; set; }
        public bool SearchInAttachments { get; set; }
    }
}
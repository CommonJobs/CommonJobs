using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    public class EmployeeEvaluation
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        //TODO: Remove from here and use directly the Employee
        public string FullName { get; set; }

        public string CurrentPosition { get; set; }

        public string Seniority { get; set; }

        public string Project { get; set; }

        //An Id
        public string Responsible { get; set; }

        //An Id
        public string Template { get; set; }

        public string Period { get; set; }
    }
}
;

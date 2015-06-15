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

        public string EmployeeName { get; set; }

        public string CurrentPosition { get; set; }

        public string Seniority { get; set; }

        //An Id
        public string Responsible { get; set; }

        //List of Ids
        public List<string> Evaluators { get; set; }

        //An Id
        public string Template { get; set; }

        public DateTime Date { get; set; }
    }
}
;

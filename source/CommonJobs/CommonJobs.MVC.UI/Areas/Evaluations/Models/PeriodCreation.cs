using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class PeriodCreation
    {
        public PeriodCreation()
        {
            Employees = new List<EmployeeEvaluation>();    
        }
        public List<EmployeeEvaluation> Employees { get; set; }

    }
}
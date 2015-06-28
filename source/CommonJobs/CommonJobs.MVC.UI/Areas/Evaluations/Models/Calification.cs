using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class Calification
    {
        public EmployeeEvaluation Employee { get; set; }
        public Template Template { get; set; }
    }
}

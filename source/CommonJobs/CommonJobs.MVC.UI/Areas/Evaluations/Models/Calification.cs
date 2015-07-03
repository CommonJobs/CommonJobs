using CommonJobs.Application.EvalForm;
using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class Calification
    {
        public int UserView { get; set; } 

        public EmployeeEvaluation Evaluation { get; set; }
        
        public Template Template { get; set; }

        public List<EvaluationCalification> Califications { get; set; }

        public string UserLogged { get; set; }
    }
}

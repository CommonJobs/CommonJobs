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
        public UserView View { get; set; } 

        public EmployeeEvaluationDTO Evaluation { get; set; }
        
        public Template Template { get; set; }

        public List<EvaluationCalification> Califications { get; set; }

        public string StrengthsComment { get; set; }

        public string ImproveComment { get; set; }

        public string ActionPlanComment { get; set; }

        public string UserLogged { get; set; }

        public enum UserView
        {
            Auto,
            Responsible,
            Evaluation
        }
    }
}

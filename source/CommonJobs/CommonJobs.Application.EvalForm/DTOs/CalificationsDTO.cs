using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.DTOs
{
    public class CalificationsDTO
    {
        public EmployeeEvaluationDTO Evaluation { get; set; }

        public List<EvaluationCalification> Califications { get; set; }

        public int UserView { get; set; }

        public string StrengthsComment { get; set; }

        public string ImproveComment { get; set; }

        public string ActionPlanComment { get; set; }
    }
}

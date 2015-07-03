using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Dtos
{
    public class CalificationsDto
    {
        public UserView View { get; set; }

        public EmployeeEvaluation Evaluation { get; set; }

        public List<EvaluationCalification> Califications { get; set; }
    }

    public enum UserView
    {
        Auto,
        Responsible,
        Evaluation
    }
}

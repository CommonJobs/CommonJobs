using CommonJobs.Application.EvalForm;
using CommonJobs.Domain.Evaluations;
using System.Collections.Generic;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class PeriodCreation
    {
        public PeriodCreation()
        {
            Employees = new List<EmployeeEvaluationDTO>();
        }
        public List<EmployeeEvaluationDTO> Employees { get; set; }

    }
}

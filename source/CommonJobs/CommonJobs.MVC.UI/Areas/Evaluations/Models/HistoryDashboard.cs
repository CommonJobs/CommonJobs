using CommonJobs.Application.EvalForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class HistoryDashboard
    {
        public HistoryDashboard()
        {
            EvaluationsItems = new List<EmployeeEvaluationDTO>();
        }
        public List<EmployeeEvaluationDTO> EvaluationsItems { get; set; }
    }
}

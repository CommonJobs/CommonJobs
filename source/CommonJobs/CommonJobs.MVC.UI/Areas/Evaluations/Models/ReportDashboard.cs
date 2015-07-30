using CommonJobs.Application.EvalForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CommonJobs.Mvc.UI.Areas.Evaluations.Models
{
    public class ReportDashboard
    {
        public ReportDashboard()
        {
            EvaluationsReport = new List<EmployeeEvaluationDTO>();
        }
        public List<EmployeeEvaluationDTO> EvaluationsReport { get; set; }
    }
}
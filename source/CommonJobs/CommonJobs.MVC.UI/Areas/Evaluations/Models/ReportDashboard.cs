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
            Evaluations = new List<EmployeeEvaluationDTO>();
        }
        public List<EmployeeEvaluationDTO> Evaluations { get; set; }
    }
}
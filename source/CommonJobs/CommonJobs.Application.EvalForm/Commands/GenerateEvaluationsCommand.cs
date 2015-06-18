using CommonJobs.Application.EvalForm;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class GenerateEvaluationsCommand : Command
    {
        private List<EmployeeEvaluation> _employeesEvaluations { get; set; }
        private string _period { get; set; }

        public GenerateEvaluationsCommand(List<EmployeeEvaluation> employeesEvaluations, string period)
        {
            _employeesEvaluations = employeesEvaluations;
            _period = period;
        }

        public override void Execute()
        {
            foreach (var e in _employeesEvaluations)
            {
                e.Period = _period;
                e.Id = Common.GenerateEvaluationId(e.Period, e.UserName);
                RavenSession.Store(e);
                //After we create the evaluation, we create the calification document for the auto-evaluation and the responsible
                ExecuteCommand(new GenerateCalificationCommand(e.Period, e.UserName, e.UserName, e.Template, CalificationType.Auto, e.Id));
                ExecuteCommand(new GenerateCalificationCommand(e.Period, e.UserName, e.Responsible, e.Template, CalificationType.Responsible, e.Id));
            }
        }
    }
}

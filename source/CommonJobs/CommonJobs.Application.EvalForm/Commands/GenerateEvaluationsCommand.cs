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

        public GenerateEvaluationsCommand(List<EmployeeEvaluation> employeesEvaluations)
        {
            _employeesEvaluations = employeesEvaluations;
        }

        public override void Execute()
        {
            foreach (var e in _employeesEvaluations)
            {
                e.Id = Common.GenerateEvaluationId(e.UserName, e.Period);
                RavenSession.Store(e);
                //After we create the evaluation, we create the calification document for the auto-evaluation and the responsible
                ExecuteCommand(new GenerateCalificationCommand(e.Period, e.UserName, e.UserName, e.Template));
                ExecuteCommand(new GenerateCalificationCommand(e.Period, e.UserName, e.Responsible, e.Template));
            }
        }
    }
}

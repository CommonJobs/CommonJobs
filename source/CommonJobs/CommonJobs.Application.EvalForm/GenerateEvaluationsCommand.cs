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

        public GenerateEvaluationsCommand(string period, List<EmployeeEvaluation> employeesEvaluations)
        {
            _employeesEvaluations = employeesEvaluations;
            _period = period;
        }

        public override void Execute()
        {
            foreach (var e in _employeesEvaluations)
            {
                e.Id = Common.GenerateEvaluationId(e.UserName, _period);
                RavenSession.Store(e);
                //After we create the evaluation, we create the calification document for the auto-evaluation and the responsible
                ExecuteCommand(new GenerateCalificationCommand(_period, e.UserName, e.UserName));
                ExecuteCommand(new GenerateCalificationCommand(_period, e.UserName, e.Responsible));
            }
        }
    }
}

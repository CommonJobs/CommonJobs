using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm
{
    public class AddEvaluatorsCommand : Command
    {
        public EmployeeEvaluation _employeeEvaluation { get; set; }
        public List<string> _evaluators { get; set; }

        public AddEvaluatorsCommand(EmployeeEvaluation employeeEvaluation, List<string> evaluators)
        {
            _employeeEvaluation = employeeEvaluation;
            _evaluators = evaluators;
        }

        public override void Execute()
        {
            _employeeEvaluation.Evaluators = _evaluators;
            RavenSession.Store(_employeeEvaluation);
        }
    }
}

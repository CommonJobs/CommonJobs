using CommonJobs.Application.EvalForm;
using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class AddEvaluatorsCommand : Command
    {
        private EmployeeEvaluation _employeeEvaluation { get; set; }
        private List<string> _evaluators { get; set; }
        private string _period { get; set; }

        public AddEvaluatorsCommand(string period, EmployeeEvaluation employeeEvaluation, List<string> evaluators)
        {
            _employeeEvaluation = employeeEvaluation;
            _evaluators = evaluators;
            _period = period;
        }

        public override void Execute()
        {
            ///TODO: INCOMPLETE COMMAND
            ///* The input list should replace the current saved list
            ///** We need to check which users are not present in this list and delete them (without taking into account the auto-evaluation and the responsible's one)

            foreach (var e in _evaluators)
            {
                ExecuteCommand(new GenerateCalificationCommand(_period, _employeeEvaluation.UserName, e));
            }
        }
    }
}

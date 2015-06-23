using CommonJobs.Application.EvalForm;
using CommonJobs.Application.EvalForm.DTOs;
using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class UpdateEvaluatorsCommand : Command
    {
        private EmployeeEvaluation _employeeEvaluation { get; set; }
        private List<EvaluatorsUpdateDTO> _evaluators { get; set; }

        public UpdateEvaluatorsCommand(EmployeeEvaluation employeeEvaluation, List<EvaluatorsUpdateDTO> evaluators)
        {
            _employeeEvaluation = employeeEvaluation;
            _evaluators = evaluators;
        }

        public override void Execute()
        {
            foreach (var e in _evaluators)
            {
                if (e.Action == EvaluatorAction.Add)
                {
                    ExecuteCommand(new GenerateCalificationCommand(_employeeEvaluation.Period, _employeeEvaluation.UserName, e.UserName, _employeeEvaluation.Template, CalificationType.Evaluator, _employeeEvaluation.Id));
                }
                else
                {
                    var id = Common.GenerateCalificationId(_employeeEvaluation.Period, _employeeEvaluation.UserName, e.UserName);
                    var calification = RavenSession.Load<EvaluationCalification>(id);
                    RavenSession.Delete(calification);
                }
            }
        }
    }
}

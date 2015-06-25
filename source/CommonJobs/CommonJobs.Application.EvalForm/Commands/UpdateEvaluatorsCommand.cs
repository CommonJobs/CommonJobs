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
        private EmployeeEvaluationDTO _employeeEvaluation { get; set; }
        private List<EvaluatorsUpdateDTO> _evaluators { get; set; }

        public UpdateEvaluatorsCommand(EmployeeEvaluationDTO employeeEvaluation, List<EvaluatorsUpdateDTO> evaluators)
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
                    //TODO: Check if e.UserName exists in the DB
                    var employee = RavenSession.Load<Employee>(e.UserName);
                    if (employee != null)
                    {
                        ExecuteCommand(new GenerateCalificationCommand(_employeeEvaluation.Period, _employeeEvaluation.UserName, e.UserName, _employeeEvaluation.TemplateId, CalificationType.Evaluator, _employeeEvaluation.Id));
                    }
                    else
                    {
                        throw new ApplicationException(string.Format("Error: Evaluador no valido: {0}.", e.UserName));
                    }
                }
                else
                {
                    var id = EvaluationCalification.GenerateCalificationId(_employeeEvaluation.Period, _employeeEvaluation.UserName, e.UserName);
                    var calification = RavenSession.Load<EvaluationCalification>(id);
                    if (calification != null) {
                        RavenSession.Delete(calification);
                    }
                }
            }
        }
    }
}

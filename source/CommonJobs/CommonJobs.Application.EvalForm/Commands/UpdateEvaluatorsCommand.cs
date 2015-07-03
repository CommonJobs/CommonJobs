using CommonJobs.Application.EvalForm;
using CommonJobs.Application.EvalForm.Dtos;
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
        private List<EvaluatorsUpdateDto> _evaluators { get; set; }

        public UpdateEvaluatorsCommand(EmployeeEvaluationDTO employeeEvaluation, List<EvaluatorsUpdateDto> evaluators)
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
                    var employee = RavenSession
                        .Query<Employee>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                        .Where(x => x.UserName == e.UserName)
                        .FirstOrDefault();
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

using CommonJobs.Application.EvalForm.Dtos;
using CommonJobs.Application.Evaluations.EmployeeSearching;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Linq;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluationCalificationsForEmployeeManager : Command<CalificationsDto>
    {
        private string _period;

        private string _evaluatedUser;

        public GetEvaluationCalificationsForEmployeeManager(string period, string evaluatedUser)
        {
            _period = period;

            _evaluatedUser = evaluatedUser;
        }

        public override CalificationsDto ExecuteWithResult()
        {
            // The employeeManager should be able to see any evaluation that has been finished.

            var evId = EmployeeEvaluation.GenerateEvaluationId(_period, _evaluatedUser);
            var evaluation = RavenSession.Load<EmployeeEvaluation>(evId);

            if (evaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación inexistente: {0}.", evId));
            }

            if (!evaluation.Finished)
            {
                throw new ApplicationException(string.Format("Error: La evaluación: {0} debe estar finalizada para poder ser vista.", evId));
            }

            Employee_Search.Projection employee = RavenSession
                .Query<Employee_Search.Projection, Employee_Search>()
                .Where(x => x.IsActive && x.UserName == evaluation.UserName)
                .FirstOrDefault();

            var califications = RavenSession
                .Query<EvaluationCalification>()
                .Where(x => x.EvaluationId == evId)
                .ToList();

            var evaluators = califications.Where(c => c.Owner == CalificationType.Evaluator).Select(c => c.EvaluatorEmployee).ToList();

            var evaluationDto = CalificationsEvaluationDto.Create(evaluation, evaluators, evaluation.CurrentPosition ?? employee.CurrentPosition, evaluation.Seniority ?? employee.Seniority);

            return new CalificationsDto()
            {
                View = UserView.Company,
                Evaluation = evaluationDto,
                Califications = califications.Where(c => _evaluatedUser == c.EvaluatedEmployee && (c.Owner == CalificationType.Auto || c.Owner == CalificationType.Company)).ToList()
            };
        }
    }
}

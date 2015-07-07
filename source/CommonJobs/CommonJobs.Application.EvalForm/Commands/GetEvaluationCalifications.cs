using CommonJobs.Application.EvalForm.Dtos;
using CommonJobs.Application.Evaluations.EmployeeSearching;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluationCalifications : Command<CalificationsDto>
    {
        private string _period;

        private string _evaluatedUser;

        private string _loggedUser;

        public GetEvaluationCalifications(string period, string evaluatedUser, string loggedUser)
        {
            _period = period;

            _loggedUser = loggedUser;

            _evaluatedUser = evaluatedUser;
        }

        public override CalificationsDto ExecuteWithResult()
        {
            var evId = EmployeeEvaluation.GenerateEvaluationId(_period, _evaluatedUser);
            var evaluation = RavenSession.Load<EmployeeEvaluation>(evId);

            if (evaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación inexistente: {0}.", evId));
            }

            Employee_Search.Projection employee = RavenSession
                .Query<Employee_Search.Projection, Employee_Search>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.IsActive && x.UserName == evaluation.UserName).FirstOrDefault();

            var evaluationDto = CalificationsEvaluationDto.Create(evaluation, employee.CurrentPosition, employee.Seniority);

            var califications = RavenSession
                .Query<EvaluationCalification>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.EvaluationId == evId).ToList();

            if (evaluation.ReadyForDevolution) // Ready for devolution
            {
                return new CalificationsDto()
                {
                    View = UserView.Company,
                    Evaluation = evaluationDto,
                    Califications = califications.Where(c => (_loggedUser == c.EvaluatorEmployee && c.EvaluatorEmployee == c.EvaluatedEmployee) || c.Owner == CalificationType.Company).ToList()
                };
            }
            else if (_evaluatedUser == _loggedUser) // Auto-evaluation
            {
                return new CalificationsDto()
                {
                    View = UserView.Auto,
                    Evaluation = evaluationDto,
                    Califications = new List<EvaluationCalification>() { califications.Single(c => _loggedUser == c.EvaluatorEmployee && c.EvaluatorEmployee == c.EvaluatedEmployee) }
                };
            }
            else if (evaluation.ResponsibleId == _loggedUser) // Responsible & Company (responsible && finished)
            {
                return new CalificationsDto()
                {
                    View = califications.Any(c => c.Owner == CalificationType.Company) ? UserView.Company : UserView.Responsible,
                    Evaluation = evaluationDto,
                    Califications = califications
                };
            }
            else // Evaluator
            {
                return new CalificationsDto()
                {
                    View = UserView.Evaluation,
                    Evaluation = evaluationDto,
                    Califications = califications.Where(c => _loggedUser == c.EvaluatorEmployee || c.Owner == CalificationType.Auto).ToList()
                };
            }
        }
    }
}

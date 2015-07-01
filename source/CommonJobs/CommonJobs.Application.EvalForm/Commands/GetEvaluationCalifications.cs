using CommonJobs.Application.EvalForm.DTOs;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluationCalifications : Command<CalificationsDTO>
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

        public override CalificationsDTO ExecuteWithResult()
        {
            var evId = EmployeeEvaluation.GenerateEvaluationId(_period, _evaluatedUser);
            var evaluation = RavenSession.Load<EmployeeEvaluation>(evId);

            if (evaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación inexistente: {0}.", evId));
            }

            RavenQueryStatistics stats;
            var califications = RavenSession
                .Query<EvaluationCalification>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.EvaluationId == evId).ToList();

            if (evaluation.ReadyForDevolution) // Ready for devolution
            {
                return new CalificationsDTO()
                {
                    View = UserView.Responsible,
                    Evaluation = evaluation,
                    Califications = califications.Where(c => (_loggedUser == c.EvaluatorEmployee && c.EvaluatorEmployee == c.EvaluatedEmployee) || c.Owner == CalificationType.Company).ToList()
                };
            }
            else if (_evaluatedUser == _loggedUser) // Auto-evaluation
            {
                return new CalificationsDTO()
                {
                    View = UserView.Auto,
                    Evaluation = evaluation,
                    Califications = new List<EvaluationCalification>() { califications.Single(c => _loggedUser == c.EvaluatorEmployee && c.EvaluatorEmployee == c.EvaluatedEmployee) }
                };
            }
            else if (evaluation.ResponsibleId == _loggedUser) // Responsible & Company (responsible && finished)
            {
                return new CalificationsDTO()
                {
                    View = UserView.Responsible,
                    Evaluation = evaluation,
                    Califications = califications
                };
            }
            else // Evaluator
            {
                return new CalificationsDTO()
                {
                    View = UserView.Evaluation,
                    Evaluation = evaluation,
                    Califications = califications.Where(c => _loggedUser == c.EvaluatorEmployee || c.Owner == CalificationType.Auto).ToList()
                };
            }
        }
    }
}

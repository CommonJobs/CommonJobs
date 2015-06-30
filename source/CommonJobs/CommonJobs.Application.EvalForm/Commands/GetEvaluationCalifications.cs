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
        private string _period { get; set; }

        private string _evaluatedUser { get; set; }

        private string _loggedUser { get; set; }

        public GetEvaluationCalifications(string period, string evaluatedUser, string loggedUser)
        {
            _period = period;

            _loggedUser = loggedUser;

            _evaluatedUser = evaluatedUser;
        }

        public override CalificationsDTO ExecuteWithResult()
        {
            var evId = EmployeeEvaluation.GenerateEvaluationId(_period, _loggedUser);
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
                .Where(x => x.EvaluationId == _period).ToList();

            var calificationsToReturn = new List<EvaluationCalification>();

            if (evaluation.OpenToDevolution) // Ready to devolution
            {
                calificationsToReturn.AddRange(califications.Where(c => (_loggedUser == c.EvaluatorEmployee && c.EvaluatorEmployee == c.EvaluatedEmployee) || c.Owner == CalificationType.Company));
            }
            else if (_evaluatedUser == _loggedUser) // Auto-evaluation
            {
                calificationsToReturn.Add(califications.Single(c => _loggedUser == c.EvaluatorEmployee && c.EvaluatorEmployee == c.EvaluatedEmployee));
            }
            else if (evaluation.ResponsibleId == _loggedUser) // Responsible & Company (responsible && finished)
            {
                calificationsToReturn = califications;
            }
            else // Evaluator
            {
                calificationsToReturn.AddRange(califications.Where(c => _loggedUser == c.EvaluatorEmployee || c.Owner == CalificationType.Auto));
            }

            return new CalificationsDTO()
                {
                    Evaluation = evaluation,
                    Califications = calificationsToReturn
                };
        }
    }
}

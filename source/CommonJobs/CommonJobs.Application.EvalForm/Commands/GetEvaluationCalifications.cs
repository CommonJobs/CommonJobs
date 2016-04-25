using CommonJobs.Application.EvalForm.Dtos;
using CommonJobs.Application.EvalForm.Indexes;
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

        private bool _isEmployeeManager;

        public GetEvaluationCalifications(string period, string evaluatedUser, string loggedUser, bool isEmployeeManager = false)
        {
            _period = period;

            _loggedUser = loggedUser;

            _evaluatedUser = evaluatedUser;

            _isEmployeeManager = isEmployeeManager;
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
                .Where(x => x.IsActive && x.UserName == evaluation.UserName).FirstOrDefault();

            var califications = RavenSession.Advanced.LoadStartingWith<EvaluationCalification>(evId + "/").ToList();

            var evaluators = califications.Where(c => c.Owner == CalificationType.Evaluator).Select(c => c.EvaluatorEmployee).ToList();

            var evaluationDto = CalificationsEvaluationDto.Create(evaluation, evaluators, evaluation.CurrentPosition ?? employee.CurrentPosition, evaluation.Seniority ?? employee.Seniority);

            if (!CanViewEvaluation(evaluationDto, califications))
            {
                throw new ApplicationException(string.Format("Error: Usuario {0} no tiene permiso para ver la evaluación {1}", _loggedUser, evId));
            }

            // If evaluation is finished
            // - Show ALWAYS company & auto

            if (evaluation.Finished)
            {
                return GetFinishedEvaluation(evaluationDto, califications);
            }

            // If loggedUser is the evaluated
            // - Show auto-eval if not RFD
            // - Show auto-eval & company if RFD

            if (_loggedUser == _evaluatedUser)
            {
                return GetAutoEvaluation(evaluationDto, califications);
            }

            // If loggedUser is responsible
            // - Show all

            if (_loggedUser == evaluationDto.ResponsibleId)
            {
                return GetFullEvaluation(evaluationDto, califications);
            }

            // If loggedUser is evaluator
            // - Show evluator eval

            return GetEvaluatorEvaluation(evaluationDto, califications);
        }

        private CalificationsDto GetEvaluatorEvaluation(CalificationsEvaluationDto evaluationDto, List<EvaluationCalification> califications)
        {
            return new CalificationsDto()
            {
                View = UserView.Evaluation,
                Evaluation = evaluationDto,
                Califications = califications.Where(c =>
                    _loggedUser == c.EvaluatorEmployee || c.Owner == CalificationType.Auto || (evaluationDto.ReadyForDevolution && c.Owner == CalificationType.Company)
                    ).ToList()
            };
        }

        private CalificationsDto GetFullEvaluation(CalificationsEvaluationDto evaluationDto, List<EvaluationCalification> califications)
        {
            return new CalificationsDto()
            {
                View = califications.Any(c => c.Owner == CalificationType.Company) ? UserView.Company : UserView.Responsible,
                Evaluation = evaluationDto,
                Califications = califications
            };
        }

        private CalificationsDto GetAutoEvaluation(CalificationsEvaluationDto evaluationDto, List<EvaluationCalification> califications)
        {
            if (evaluationDto.ReadyForDevolution)
            {
                return new CalificationsDto()
                {
                    View = UserView.Auto,
                    Evaluation = evaluationDto,
                    Califications = califications.Where(c => _evaluatedUser == c.EvaluatedEmployee && (c.Owner == CalificationType.Auto || c.Owner == CalificationType.Company)).ToList()
                };
            }
            else
            {
                return new CalificationsDto()
                {
                    View = UserView.Auto,
                    Evaluation = evaluationDto,
                    Califications = califications.Where(c => _evaluatedUser == c.EvaluatedEmployee && c.Owner == CalificationType.Auto).ToList()
                };
            }
        }

        private CalificationsDto GetFinishedEvaluation(CalificationsEvaluationDto evaluationDto, List<EvaluationCalification> califications)
        {
            return new CalificationsDto()
            {
                View = (_loggedUser == evaluationDto.ResponsibleId || _isEmployeeManager)
                ? UserView.Company
                    : (_loggedUser == _evaluatedUser)
                        ? UserView.Auto
                        : UserView.Evaluation,
                Evaluation = evaluationDto,
                Califications = califications.Where(c => _evaluatedUser == c.EvaluatedEmployee && (c.Owner == CalificationType.Auto || c.Owner == CalificationType.Company)).ToList()
            };
        }

        /// <summary>
        /// Returns true if the logged user is the one being evaluated, the responsible or an additional evaluator
        /// </summary>
        /// <param name="evaluationDto"></param>
        /// <param name="califications"></param>
        /// <returns></returns>
        private bool CanViewEvaluation(CalificationsEvaluationDto evaluationDto, List<EvaluationCalification> califications)
        {
            return evaluationDto.Finished && _isEmployeeManager
                || _loggedUser == evaluationDto.UserName
                || _loggedUser == evaluationDto.ResponsibleId
                || califications.Any(c => c.EvaluatorEmployee == _loggedUser);
        }
    }
}

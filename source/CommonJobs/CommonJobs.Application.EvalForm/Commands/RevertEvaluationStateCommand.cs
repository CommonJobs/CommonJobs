using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Application.EvalForm.Helper;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class RevertEvaluationStateCommand : Command
    {
        private string _evaluatedEmployee;
        private string _period;
        private string _action;

        public RevertEvaluationStateCommand(string period, string evaluatedEmployee, string action)
        {
            _period = period;
            _evaluatedEmployee = evaluatedEmployee;
            _action = action;
        }

        public override void Execute()
        {
            var evaluationId = EmployeeEvaluation.GenerateEvaluationId(_period, _evaluatedEmployee);
            var evaluation = RavenSession.Load<EmployeeEvaluation>(evaluationId);
            var califications = RavenSession.Advanced.LoadStartingWith<EvaluationCalification>(evaluationId + "/").ToList();
            var isResponsibleEvalFinished = califications.Any(x => x.Owner == CalificationType.Responsible && x.Finished);
            var isCompanyEvalFinished = califications.Any(x => x.Owner == CalificationType.Company && x.Finished);
            var isAutoEvalFinished = califications.Any(x => x.Owner == CalificationType.Auto && x.Finished);
            var isEvaluatorEvalFinished = califications.Any(x => x.Owner == CalificationType.Evaluator && x.Finished);
            var action = RevertEvaluationActionsHelper.TryParse(_action);

            if (!RevertEvaluationActionsHelper.FindPosibleRevertAction(
                evaluation.Finished,
                evaluation.ReadyForDevolution,
                isCompanyEvalFinished,
                isResponsibleEvalFinished,
                isAutoEvalFinished,
                isEvaluatorEvalFinished,
                action))
            {
                throw new ApplicationException("Action not allowed");
            }
            var calificationsByType = califications.ToLookup(x => x.Owner);

            switch (action)
            {
                case RevertAction.ReopenForDevolution:
                    evaluation.Finished = false;
                    break;

                case RevertAction.CancelDevolution:
                    evaluation.ReadyForDevolution = false;
                    break;

                case RevertAction.ReopenEvalCompany:
                    calificationsByType[CalificationType.Company].First().Finished = false;
                    break;

                case RevertAction.ReopenEvalResponsible:
                    calificationsByType[CalificationType.Responsible].First().Finished = false;
                    RavenSession.Delete(calificationsByType[CalificationType.Company].First());
                    break;

                case RevertAction.ReopenAutoEvaluation:
                    calificationsByType[CalificationType.Auto].First().Finished = false;
                    break;

                case RevertAction.ReopenEvalEvaluators:
                    foreach (var calification in calificationsByType[CalificationType.Evaluator])
                    {
                        calification.Finished = false;
                    }
                    break;
            }
        }
    }
}

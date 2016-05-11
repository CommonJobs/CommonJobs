using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Helper
{
    public static class RevertEvaluationActionsHelper
    {
        public enum RevertAction
        {
            ReopenForDevolution,
            CancelDevolution,
            ReopenEvalCompany,
            ReopenEvalResponsible,
            ReopenEvalEvaluators,
            ReopenAutoEvaluation
        }

        public static IEnumerable<RevertAction> GetPosibleRevertActions(EmployeeEvaluation evaluation, IEnumerable<EvaluationCalification> califications)
        {
            var calificationsByType = califications.ToLookup(x => x.Owner);
            if (evaluation.Finished)
            {
                yield return RevertAction.ReopenForDevolution;
            }

            if (evaluation.ReadyForDevolution)
            {
                yield return RevertAction.CancelDevolution;
            }

            if (!evaluation.Finished
                && !evaluation.ReadyForDevolution
                // TODO: verify if it is possible to have a null on calificationsByType[CalificationType.Company]
                && calificationsByType[CalificationType.Company] != null
                && calificationsByType[CalificationType.Company].Any(x => x.Finished))
            {
                yield return RevertAction.ReopenEvalCompany;
            }

            if (!evaluation.Finished
               && !evaluation.ReadyForDevolution
               && calificationsByType[CalificationType.Auto].First().Finished)
            {
                yield return RevertAction.ReopenAutoEvaluation;
            }

            if (!evaluation.Finished
               && !evaluation.ReadyForDevolution
               && calificationsByType[CalificationType.Company] != null
               && calificationsByType[CalificationType.Company].All(x => !x.Finished)
               && calificationsByType[CalificationType.Responsible] != null
               && calificationsByType[CalificationType.Responsible].Any(x => x.Finished))
            {
                yield return RevertAction.ReopenEvalResponsible;
            }

            if (!evaluation.Finished
               && !evaluation.ReadyForDevolution
                && (calificationsByType[CalificationType.Company] == null || calificationsByType[CalificationType.Company].All(x => !x.Finished))
               && calificationsByType[CalificationType.Evaluator] != null
               && calificationsByType[CalificationType.Evaluator].Any(x => x.Finished))
            {
                yield return RevertAction.ReopenEvalEvaluators;
            }
        }
    }
}

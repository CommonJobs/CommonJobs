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

        public static string MapRevertActionName(RevertAction action)
        {
            switch (action)
            {
                case RevertAction.ReopenForDevolution:
                    return "Reabrir para devolución";
                case RevertAction.CancelDevolution:
                    return "Suspender devolución";
                case RevertAction.ReopenEvalCompany:
                    return "Reabrir Eval Empresa";
                case RevertAction.ReopenEvalResponsible:
                    return "Reabrir Eval Responsable";
                case RevertAction.ReopenEvalEvaluators:
                    return "Reabrir Eval Evaluadores";
                case RevertAction.ReopenAutoEvaluation:
                    return "Reabrir Auto-evaluación";
            }
            return null;
        }

        public static IEnumerable<RevertAction> GetPosibleRevertActions(
            bool isFinished, 
            bool isReadyForDevolution, 
            bool isCompanyEvalFinished, 
            bool isResponsibleEvalFinished, 
            bool isAutoEvalFinished, 
            bool isEvaluatorEvalFinished)
        {
            if (isFinished)
            {
                yield return RevertAction.ReopenForDevolution;
            }

            if (isReadyForDevolution)
            {
                yield return RevertAction.CancelDevolution;
            }

            if (!isFinished
                && !isReadyForDevolution
                // TODO: verify if it is possible to have a null on calificationsByType[CalificationType.Company]
                && isCompanyEvalFinished)
            {
                yield return RevertAction.ReopenEvalCompany;
            }

            if (!isFinished
               && !isReadyForDevolution
               && isAutoEvalFinished)
            {
                yield return RevertAction.ReopenAutoEvaluation;
            }

            if (!isFinished
               && !isReadyForDevolution
               && !isCompanyEvalFinished
               && isResponsibleEvalFinished)
            {
                yield return RevertAction.ReopenEvalResponsible;
            }

            if (!isFinished
               && !isReadyForDevolution
               && !isCompanyEvalFinished
               && isEvaluatorEvalFinished)
            {
                yield return RevertAction.ReopenEvalEvaluators;
            }
        }
    }
}

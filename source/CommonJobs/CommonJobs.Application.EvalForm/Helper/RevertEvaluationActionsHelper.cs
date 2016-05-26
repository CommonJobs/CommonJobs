using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Helper
{
    public enum RevertAction
    {
        [Description("Reabrir para devolución")]
        ReopenForDevolution,
        [Description("Suspender devolución")]
        CancelDevolution,
        [Description("Reabrir Eval Empresa")]
        ReopenEvalCompany,
        [Description("Reabrir Eval Responsable")]
        ReopenEvalResponsible,
        [Description("Reabrir Eval Evaluadores")]
        ReopenEvalEvaluators,
        [Description("Reabrir Auto-evaluación")]
        ReopenAutoEvaluation
    }
    public static class RevertEvaluationActionsHelper
    {
        public static IEnumerable<RevertAction> GetPosibleRevertActions(
            bool isFinished,
            bool isDevolutionInProgress,
            bool isCompanyEvalFinished,
            bool isResponsibleEvalFinished,
            bool isAutoEvalFinished,
            bool isEvaluatorEvalFinished)
        {
            if (isFinished)
            {
                yield return RevertAction.ReopenForDevolution;
            }

            if (isDevolutionInProgress)
            {
                yield return RevertAction.CancelDevolution;
            }

            if (!isFinished
                && !isDevolutionInProgress
                && isCompanyEvalFinished)
            {
                yield return RevertAction.ReopenEvalCompany;
            }

            if (!isFinished
               && !isDevolutionInProgress
               && isAutoEvalFinished)
            {
                yield return RevertAction.ReopenAutoEvaluation;
            }

            if (!isFinished
               && !isDevolutionInProgress
               && !isCompanyEvalFinished
               && isResponsibleEvalFinished)
            {
                yield return RevertAction.ReopenEvalResponsible;
            }

            if (!isFinished
               && !isDevolutionInProgress
               && !isCompanyEvalFinished
               && isEvaluatorEvalFinished)
            {
                yield return RevertAction.ReopenEvalEvaluators;
            }
        }

        public static bool FindPosibleRevertAction(
            bool finished,
            bool readyForDevolution,
            bool isCompanyEvalFinished,
            bool isResponsibleEvalFinished,
            bool isAutoEvalFinished,
            bool isEvaluatorEvalFinished,
            RevertAction action)
        {
            return GetPosibleRevertActions(finished,
                readyForDevolution,
                isCompanyEvalFinished,
                isResponsibleEvalFinished,
                isAutoEvalFinished,
                isEvaluatorEvalFinished)
                .Contains(action);
        }

        public static RevertAction TryParse(string action)
        {
            RevertAction revertAction;
            if (Enum.TryParse(action, out revertAction))
            {
                return revertAction;
            }
            throw new ApplicationException("Acción inválida");
        }

        public static string GetDescription<TEnum>(this TEnum EnumValue) where TEnum : struct
        {
            return GetEnumDescription((Enum)(object)((TEnum)EnumValue));
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}

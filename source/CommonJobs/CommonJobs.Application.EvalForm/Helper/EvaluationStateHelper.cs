using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Helper
{
    public enum EvaluationState
    {
        InProgress,
        WaitingAuto,
        WaitingResponsible,
        WaitingCompany,
        ReadyForDevolution,
        OpenForDevolution,
        Finished
    }

    public static class EvaluationStateHelper
    {
        public static EvaluationState GetEvaluationState(bool autoEvaluationDone, bool responsibleEvaluationDone, bool companyEvaluationDone, bool openToDevolution, bool finished)
        {
            if (finished) return EvaluationState.Finished;

            if (openToDevolution) return EvaluationState.OpenForDevolution;

            if (autoEvaluationDone && responsibleEvaluationDone && companyEvaluationDone) return EvaluationState.ReadyForDevolution;

            if (responsibleEvaluationDone && companyEvaluationDone) return EvaluationState.WaitingAuto;

            if (autoEvaluationDone && responsibleEvaluationDone) return EvaluationState.WaitingCompany;

            if (responsibleEvaluationDone) return EvaluationState.WaitingAuto;

            if (autoEvaluationDone) return EvaluationState.WaitingResponsible;

            return EvaluationState.InProgress;
        }
    }
}

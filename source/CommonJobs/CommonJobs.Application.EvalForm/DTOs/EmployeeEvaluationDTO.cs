using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm
{
    public class EmployeeEvaluationDTO
    {
        public bool IsResponsible { get; set; }

        public string ResponsibleId { get; set; }

        public string FullName { get; set; }

        public string CurrentPosition { get; set; }

        public string Seniority { get; set; }

        public List<string> Evaluators { get; set; }

        public EvaluationState State { get; set; }

        public string UserName { get; set; }

        public string Period { get; set; }

        public string TemplateId { get; set; }

        public string Id { get; set; }

        public bool IsEditable { get; set; }
    }

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
}

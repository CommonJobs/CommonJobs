using CommonJobs.Domain.Evaluations;
using System.Collections.Generic;

namespace CommonJobs.Application.EvalForm.Dtos
{
    public class CalificationsEvaluationDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string CurrentPosition { get; set; }

        public string Seniority { get; set; }

        public string Project { get; set; }

        public string ResponsibleId { get; set; }

        public string TemplateId { get; set; }

        public string Period { get; set; }

        public bool DevolutionInProgress { get; set; }

        public bool Finished { get; set; }

        public string StrengthsComment { get; set; }

        public string ToImproveComment { get; set; }

        public string ActionPlanComment { get; set; }

        public List<string> Evaluators { get; set; }

        public bool IsCompanyEvaluationDone { get; set; }

        public static CalificationsEvaluationDto Create(EmployeeEvaluation evaluation, List<string> evaluators, string currentPosition, string seniority, bool companyEvaluationDone)
        {
            return new CalificationsEvaluationDto()
            {
                Id = evaluation.Id,
                UserName = evaluation.UserName,
                FullName = evaluation.FullName,
                CurrentPosition = currentPosition,
                Seniority = seniority,
                Project = evaluation.Project,
                ResponsibleId = evaluation.ResponsibleId,
                TemplateId = evaluation.TemplateId,
                Period = evaluation.Period,
                DevolutionInProgress = evaluation.ReadyForDevolution,
                Finished = evaluation.Finished,
                StrengthsComment = evaluation.StrengthsComment,
                ToImproveComment = evaluation.ToImproveComment,
                ActionPlanComment = evaluation.ActionPlanComment,
                Evaluators = evaluators,
                IsCompanyEvaluationDone = companyEvaluationDone
            };
        }
    }
}

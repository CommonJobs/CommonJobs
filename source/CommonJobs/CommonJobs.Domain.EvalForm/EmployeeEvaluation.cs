using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    public class EmployeeEvaluation
    {
        public string Id
        {
            get { return GenerateEvaluationId(Period, UserName); }
        }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Project { get; set; }

        public string ResponsibleId { get; set; }

        public string TemplateId { get; set; }

        public string Period { get; set; }

        public bool ReadyForDevolution { get; set; }

        public bool Finished { get; set; }

        public string StrengthsComment { get; set; }

        public string ToImproveComment { get; set; }

        public string ActionPlanComment { get; set; }

        public static string GenerateEvaluationId(string period, string userName)
        {
            return string.Format("Evaluations/{0}/{1}", period, userName);
        }
    }
}


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

        //TODO: Remove from here and use directly the Employee
        public string FullName { get; set; }

        public string CurrentPosition { get; set; }

        public string Seniority { get; set; }

        public string Project { get; set; }

        public string ResponsibleId { get; set; }

        public string TemplateId { get; set; }

        public string Period { get; set; }

        public bool OpenToDevolution { get; set; }

        public bool Finished { get; set; }

        public static string GenerateEvaluationId(string period, string userName)
        {
            return string.Format("Evaluations/{0}/{1}", period, userName);
        }
    }
}


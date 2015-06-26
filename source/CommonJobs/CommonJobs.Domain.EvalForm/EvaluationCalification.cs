using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    /// <summary>
    /// There are four types of califications:
    /// Auto: The auto-evaluation
    /// Responsible: The responsible's evaluation
    /// Evaluator: The added evaluators evaluations
    /// Company: The company evaluation
    /// </summary>
    public enum CalificationType
    {
        Auto,
        Responsible,
        Evaluator,
        Company
    }

    public class EvaluationCalification
    {
        public string Id
        {
            get { return GenerateCalificationId(Period, EvaluatedEmployee, EvaluatorEmployee); }
        }

        public CalificationType Owner { get; set; }

        public string EvaluationId { get; set; }

        public string Period { get; set; }

        public string TemplateId { get; set; }

        public string EvaluatedEmployee { get; set; }

        public string EvaluatorEmployee { get; set; }

        //public string StrengthsComment { get; set; }

        //public string ImproveComment { get; set; }

        //public string ActionPlanComment { get; set; }

        public string Comments { get; set; }

        public List<KeyValuePair<string, decimal>> Califications { get; set; }

        public bool Finished { get; set; }

        public static string GenerateCalificationId(string period, string userName, string evaluator)
        {
            return string.Format("Evaluations/{0}/{1}/{2}", period, userName, evaluator);
        }
    }
}

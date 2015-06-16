using CommonJobs.Domain.Evaluations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    public class EvaluationCalification
    {
        public string Id { get; set; }

        //TODO: It's necessary? We have it in the EmployeeEvaluation object
        public string Period { get; set; }

        public string EvaluatedEmployee { get; set; }

        public string EvaluatorEmployee { get; set; }

        /// Evaluation fields

        public string StrengthsComment { get; set; }

        public string ImproveComment { get; set; }

        public string ActionPlanComment { get; set; }

        //Won't be always used
        public string EvaluatedEmployeeComment { get; set; }

        public List<KeyValuePair<string, decimal>> Califications { get; set; }        
    }
}

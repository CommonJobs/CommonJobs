using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.DTOs
{
    public class UpdateEvaluationDTO
    {
        public string EvaluationId { get; set; }

        public string Project { get; set; }

        public string ToImprove { get; set; }

        public string Strengths { get; set; }

        public string ActionPlan { get; set; }

        public List<UpdateCalificationDTO> Califications { get; set; }

        public bool Finished { get; set; }
    }

    public class UpdateCalificationDTO
    {
        public string CalificationId { get; set; }

        public List<KeyValuePair<string, decimal>> Items { get; set; }

        public string Comments { get; set; }
    }
}

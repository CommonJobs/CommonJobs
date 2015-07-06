using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Dtos
{
    public class UpdateEvaluationDto
    {
        public string EvaluationId { get; set; }

        public string Project { get; set; }

        public string ToImprove { get; set; }

        public string Strengths { get; set; }

        public string ActionPlan { get; set; }

        public List<UpdateCalificationDto> Califications { get; set; }

        public bool CalificationFinished { get; set; }
    }
}

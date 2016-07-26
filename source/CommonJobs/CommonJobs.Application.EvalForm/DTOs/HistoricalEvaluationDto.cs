using CommonJobs.Application.EvalForm.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm
{
    public class HistoricalEvaluationDto
    {
        public string ResponsibleId { get; set; }

        public string FullName { get; set; }

        public List<string> Evaluators { get; set; }

        public EvaluationState State { get; set; }

        public string UserName { get; set; }

        public string Period { get; set; }

        public string Id { get; set; }

        public decimal? AverageCalification { get; set; }
    }
}

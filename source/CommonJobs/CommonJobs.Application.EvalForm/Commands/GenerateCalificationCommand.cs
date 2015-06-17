using CommonJobs.Application.Evaluations;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm
{
    public class GenerateCalificationCommand : Command
    {
        private string _period { get; set; }
        private string _evaluated { get; set; }
        private string _evaluator { get; set; }
        private string _template { get; set; }

        public GenerateCalificationCommand(string period, string evaluated, string evaluator, string template)
        {
            _period = period;
            _evaluated = evaluated;
            _evaluator = evaluator;
            _template = template;
        }

        public override void Execute()
        {
            var calification = new EvaluationCalification()
            {
                Id = Common.GenerateCalificationId(_period, _evaluated, _evaluator),
                Period = _period,
                EvaluatedEmployee = _evaluated,
                EvaluatorEmployee = _evaluator,
                Template = _template
            };

            RavenSession.Store(calification);
        }
    }
}

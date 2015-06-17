using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class UpdateTemplateCommand : Command
    {
        public Template _template { get; set; }

        public UpdateTemplateCommand(Template template)
        {
            _template = template;
        }

        public override void Execute()
        {
            RavenSession.Store(_template);
        }
    }
}

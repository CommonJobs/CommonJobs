﻿using CommonJobs.Application.EvalForm;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class GenerateEvaluationsCommand : Command
    {
        private List<EmployeeEvaluation> _employeesEvaluations { get; set; }

        public GenerateEvaluationsCommand(List<EmployeeEvaluation> employeesEvaluations)
        {
            _employeesEvaluations = employeesEvaluations;
        }

        public override void Execute()
        {
            foreach (var e in _employeesEvaluations)
            {
                //Both Period and TemplateId will be fixed for this release
                e.Period = "2015-06";
                e.TemplateId = Template.DefaultTemplateId;
                //---

                RavenSession.Store(e);
                //After we create the evaluation, we create the calification document for the auto-evaluation and the responsible
                ExecuteCommand(new GenerateCalificationCommand(e.Period, e.UserName, e.UserName, e.TemplateId, CalificationType.Auto, e.Id));
                ExecuteCommand(new GenerateCalificationCommand(e.Period, e.UserName, e.ResponsibleId, e.TemplateId, CalificationType.Responsible, e.Id));
            }
        }
    }
}

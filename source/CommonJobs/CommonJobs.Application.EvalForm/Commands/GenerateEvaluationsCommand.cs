using CommonJobs.Application.EvalForm;
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
        private List<EmployeeEvaluationDTO> _employeesEvaluations { get; set; }

        public GenerateEvaluationsCommand(List<EmployeeEvaluationDTO> employeesEvaluations)
        {
            _employeesEvaluations = employeesEvaluations;
        }

        public override void Execute()
        {
            foreach (var e in _employeesEvaluations)
            {
                EmployeeEvaluation employeeEvaluation = new EmployeeEvaluation();

                //Both Period and TemplateId will be fixed for this release
                employeeEvaluation.Period = "2015-06";
                employeeEvaluation.TemplateId = Template.DefaultTemplateId;
                //---
                employeeEvaluation.UserName = e.UserName;
                employeeEvaluation.ResponsibleId = e.ResponsibleId;
                employeeEvaluation.FullName = e.FullName;
                //employeeEvaluation.Seniority will not be stored at this point
                //employeeEvaluation.CurrentPosition will not be stored at this point

                RavenSession.Store(employeeEvaluation);

                //After we create the evaluation, we create the calification document for the auto-evaluation and the responsible
                ExecuteCommand(new GenerateCalificationCommand(employeeEvaluation.Period, employeeEvaluation.UserName, employeeEvaluation.UserName, employeeEvaluation.TemplateId, CalificationType.Auto, employeeEvaluation.Id));
                ExecuteCommand(new GenerateCalificationCommand(employeeEvaluation.Period, employeeEvaluation.UserName, employeeEvaluation.ResponsibleId, employeeEvaluation.TemplateId, CalificationType.Responsible, employeeEvaluation.Id));
            }
        }
    }
}

using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    public class ProcessGenerateEvaluations : Command
    {
        public List<EmployeeEvaluation> EmployeesToEval { get; private set; }

        public ProcessGenerateEvaluations(List<EmployeeEvaluation> employeesToEval)
        {
            EmployeesToEval = employeesToEval;
        }

        public override void Execute()
        {
            //EmployeeMenu.Id = Common.GenerateEmployeeMenuId(EmployeeMenu.UserName);
            //RavenSession.Store(EmployeeMenu);
        }
    }
}

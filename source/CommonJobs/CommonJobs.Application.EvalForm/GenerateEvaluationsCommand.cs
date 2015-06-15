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
        public List<EmployeeEvaluation> _employeesEvaluations { get; private set; }

        public GenerateEvaluationsCommand(List<EmployeeEvaluation> employeesEvaluations)
        {
            _employeesEvaluations = employeesEvaluations;
        }

        public override void Execute()
        {
            foreach (var e in _employeesEvaluations)
            {
                e.Id = Common.GenerateEvaluationId(e.UserName);
                RavenSession.Store(e);
            }
        }
    }
}

using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.Evaluations
{
    /// <summary>
    /// Command for getting the list of employees to populate the evaluation generation screen
    /// </summary>
    public class GetEmployeesForEvaluationCommand : Command<List<EmployeeEvaluation>>
    {
        public override List<EmployeeEvaluation> ExecuteWithResult()
        {
            var employeesToEval = new List<EmployeeEvaluation>();

            RavenQueryStatistics stats;

            var employee = RavenSession
                .Query<Employee>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            foreach (var e in employee)
            {
                employeesToEval.Add(new EmployeeEvaluation()
                {
                    UserName = e.UserName,
                    EmployeeName = e.FullName,
                    Seniority = e.Seniority,
                    CurrentPosition = e.CurrentPosition
                });
            }

            return employeesToEval;
        }
    }
}

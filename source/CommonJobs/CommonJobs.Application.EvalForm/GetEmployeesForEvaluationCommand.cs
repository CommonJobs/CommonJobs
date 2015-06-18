using CommonJobs.Application.Evaluations.EmployeeSearching;
using CommonJobs.Utilities;
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
        private string _period;

        public GetEmployeesForEvaluationCommand(string period)
        {
            _period = period;
        }

        public override List<EmployeeEvaluation> ExecuteWithResult()
        {
            RavenQueryStatistics stats;
            IQueryable<Employee_Search.Projection> query = RavenSession
                .Query<Employee_Search.Projection, Employee_Search>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            query = query.Where(x => x.IsActive);

            var employeesProjection = query.ToList();

            var employeesToEval = employeesProjection.Select(e =>
            {
                var period = e.EvaluationPeriods.EmptyIfNull().Where(x => x.Period == _period).FirstOrDefault();
                return new EmployeeEvaluation()
                {
                    UserName = e.UserName,
                    Period = period == null ? null : period.Period,
                    Responsible = period == null ? null : period.Responsible,
                    EmployeeName = e.FirstName + ", " + e.LastName,
                    Seniority = e.Seniority,
                    CurrentPosition = e.CurrentPosition
                };
            }).ToList();
            return employeesToEval;
        }
    }
}

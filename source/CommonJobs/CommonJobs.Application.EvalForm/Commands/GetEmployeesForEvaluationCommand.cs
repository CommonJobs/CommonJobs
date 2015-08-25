using CommonJobs.Application.Evaluations.EmployeeSearching;
using CommonJobs.Utilities;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System.Collections.Generic;
using System.Linq;
using CommonJobs.Application.EvalForm;
using System;
using Raven.Client;

namespace CommonJobs.Application.Evaluations
{
    /// <summary>
    /// Command for getting the list of employees to populate the evaluation generation screen
    /// </summary>
    public class GetEmployeesForEvaluationCommand : Command<List<EmployeeEvaluationDTO>>
    {
        private string _period;

        public GetEmployeesForEvaluationCommand(string period)
        {
            _period = period;
        }

        public override List<EmployeeEvaluationDTO> ExecuteWithResult()
        {
            RavenQueryStatistics stats;
            IQueryable<Employee_Search.Projection> query = RavenSession
                .Query<Employee_Search.Projection, Employee_Search>()
                .Statistics(out stats)
                .Take(1024);

            if (stats.TotalResults > 1024)
            {
                throw new ApplicationException(string.Format("Error: Número demasiado elevado de empleados: {0}. Póngase en contacto con HelpDesk.", stats.TotalResults));
            }

            query = query.Where(x => x.IsActive);

            var employeesProjection = query.ToList();

            var employeesToEval = employeesProjection.Select(e =>
            {
                var period = e.EvaluationPeriods.EmptyIfNull().Where(x => x.Period == _period).FirstOrDefault();
                return new EmployeeEvaluationDTO()
                {
                    UserName = e.UserName,
                    Period = period == null ? null : period.Period,
                    ResponsibleId = period == null ? null : period.Responsible,
                    FullName = e.LastName + ", " + e.FirstName,
                    Seniority = e.Seniority,
                    CurrentPosition = e.CurrentPosition
                };
            }).ToList();
            return employeesToEval;
        }
    }
}

using CommonJobs.Application.EmployeeSearching;
using CommonJobs.Application.EvalForm.Helper;
using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluatedEmployees : Command<List<EmployeeEvaluationDTO>>
    {
        /// <summary>
        /// This will be an optional field. It represents the Period to filter the results.
        /// If null will not be used.
        /// </summary>
        public string Period { get; set; }

        public GetEvaluatedEmployees()
        {

        }

        public override List<EmployeeEvaluationDTO> ExecuteWithResult()
        {
            IRavenQueryable<EmployeeToEvaluate_Search.Projection> query = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>();

            if (Period != null)
            {
                query = query.Where(e => e.Period == Period);
            }

            var employeesProjection = query.ToList();
            var mapper = new EmployeeEvaluationHelper(RavenSession, null);
            return mapper.MapEmployeeEvaluation(employeesProjection);
        }
    }
}

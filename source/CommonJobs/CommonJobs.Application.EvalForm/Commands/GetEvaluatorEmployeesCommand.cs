using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Application.EvalForm.Helper;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Application.EmployeeSearching;
using CommonJobs.Domain;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluatorEmployeesCommand : Command<List<EmployeeEvaluationDTO>>
    {
        private string _loggedUser { get; set; }
        private string _period { get; set; }

        public GetEvaluatorEmployeesCommand(string loggedUser, string period)
        {
            _loggedUser = loggedUser;
            _period = period;
        }

        public override List<EmployeeEvaluationDTO> ExecuteWithResult()
        {
            RavenQueryStatistics stats;
            IQueryable<EmployeeToEvaluate_Search.Projection> query = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => (e.Period == _period)
                && (e.UserName == _loggedUser
                || e.ResponsibleId == _loggedUser
                || e.Evaluators.Any(ev => ev == _loggedUser)));

            var employeesProjection = query.ToList();
            var mapper = new EmployeeEvaluationHelper(RavenSession, _loggedUser);
            return mapper.MapEmployeeEvaluation(employeesProjection);
        }
    }
}

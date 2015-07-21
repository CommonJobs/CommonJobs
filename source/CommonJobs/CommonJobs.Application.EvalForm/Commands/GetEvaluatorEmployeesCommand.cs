using CommonJobs.Application.EvalForm.EmployeeSearching;
using CommonJobs.Application.Evaluations;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEvaluatorEmployeesCommand : Command<List<EmployeeEvaluationDTO>>
    {
        private string _loggedUser { get; set; }

        public GetEvaluatorEmployeesCommand(string loggedUser)
        {
            _loggedUser = loggedUser;
        }

        public override List<EmployeeEvaluationDTO> ExecuteWithResult()
        {
            RavenQueryStatistics stats;
            IQueryable<EmployeeToEvaluate_Search.Projection> query = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => e.ResponsibleId == _loggedUser || e.Evaluators.Any(ev => ev == _loggedUser));

            var employeesProjection = query.ToList();

            var employeesForResponsible = employeesProjection.Select(e =>
            {
                return new EmployeeEvaluationDTO()
                {
                    IsResponsible = e.ResponsibleId == _loggedUser,
                    ResponsibleId = e.ResponsibleId,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Period = e.Period,
                    CurrentPosition = e.CurrentPosition,
                    Seniority = e.Seniority,
                    Evaluators = e.Evaluators != null ? e.Evaluators.ToList() : new List<string>(),
                    State = EmployeeEvaluationDTO.GetEvaluationState(e.AutoEvaluationDone, e.ResponsibleEvaluationDone, e.CompanyEvaluationDone, e.OpenToDevolution, e.Finished),
                    Id = e.Id,
                    TemplateId = e.TemplateId,
                    IsEditable = getEvaluationEditable(e)
                };
            }).ToList();

            return employeesForResponsible;
        }

        private bool getEvaluationEditable(EmployeeToEvaluate_Search.Projection projection)
        {
            if (projection.ResponsibleId == _loggedUser)
            {
                return !projection.Finished;
            }
            return projection.CalificationsState.Any(e => e.UserName == _loggedUser && !e.Finished);
        }
    }
}

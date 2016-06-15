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
            var employeUserNames = employeesProjection.Select(e => e.UserName);
            var employee = RavenSession
                .Query<Employee, EmployeeByUserName_Search>()
                .Where(x => x.UserName.In(employeUserNames))
                .ToDictionary(k => k.UserName);
            var employeesForResponsible = employeesProjection.Select(e =>
            {
                return new EmployeeEvaluationDTO()
                {
                    IsResponsible = e.ResponsibleId == _loggedUser,
                    ResponsibleId = e.ResponsibleId,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Period = e.Period,
                    CurrentPosition = employee[e.UserName].CurrentPosition,
                    Seniority = employee[e.UserName].Seniority,
                    Evaluators = e.Evaluators != null ? e.Evaluators.ToList() : new List<string>(),
                    State = EvaluationStateHelper.GetEvaluationState(e.AutoEvaluationDone, e.ResponsibleEvaluationDone, e.CompanyEvaluationDone, e.OpenToDevolution, e.Finished),
                    Id = e.Id,
                    TemplateId = e.TemplateId,
                    IsEditable = getEvaluationEditable(e),
                    PosibleRevertActions = RevertEvaluationActionsHelper.GetPosibleRevertActions(
                        e.Finished,
                        e.OpenToDevolution,
                        e.CompanyEvaluationDone,
                        e.ResponsibleEvaluationDone,
                        e.AutoEvaluationDone,
                        e.AnyEvaluatorEvaluationDone)
                        .Select(x => new PosibleRevertActions { ActionName = x.GetDescription(), ActionValue = x.ToString() })
                        .ToList()
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

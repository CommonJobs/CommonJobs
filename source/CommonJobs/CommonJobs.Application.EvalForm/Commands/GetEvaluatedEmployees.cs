using CommonJobs.Application.EvalForm.EmployeeSearching;
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
        private string _period;

        public GetEvaluatedEmployees(string period)
        {
            _period = period;
        }

        public override List<EmployeeEvaluationDTO> ExecuteWithResult()
        {
            //3. If user is EmployeeManager

            //var sessionRoles = (string[])HttpContext.Session[CommonJobs.Mvc.UI.Controllers.AccountController.SessionRolesKey] ?? new string[] { };
            //var required = new List<string>() { "EmployeeManagers" };
            //var isManager = sessionRoles.Intersect(required).Any();

            RavenQueryStatistics stats;
            IQueryable<EmployeeToEvaluate_Search.Projection> query = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => e.Period == _period);

            var employeesProjection = query.ToList();

            var employeesForResponsible = employeesProjection.Select(e =>
            {
                return new EmployeeEvaluationDTO()
                {
                    ResponsibleId = e.ResponsibleId,
                    FullName = e.FullName,
                    UserName = e.UserName,
                    Period = e.Period,
                    CurrentPosition = e.CurrentPosition,
                    Seniority = e.Seniority,
                    Evaluators = e.Evaluators != null ? e.Evaluators.ToList() : new List<string>(),
                    State = EmployeeEvaluationDTO.GetEvaluationState(e.AutoEvaluationDone, e.ResponsibleEvaluationDone, e.CompanyEvaluationDone, e.OpenToDevolution, e.Finished),
                    Id = e.Id,
                    TemplateId = e.TemplateId
                };
            }).ToList();

            return employeesForResponsible;
        }
    }
}

using CommonJobs.Application.EmployeeSearching;
using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Domain;
using Raven.Client;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Helper
{
    public class EmployeeEvaluationHelper
    {
        private IDocumentSession _ravenSession;
        private string _loggedUser;

        public EmployeeEvaluationHelper(IDocumentSession ravenSession, string loggedUser)
        {
            _ravenSession = ravenSession;
            _loggedUser = loggedUser;
        }

        public List<EmployeeEvaluationDTO> MapEmployeeEvaluation(List<EmployeeToEvaluate_Search.Projection> employeesProjection)
        {
            var employeUserNames = employeesProjection.Select(e => e.UserName);
            var employeeByUsername = _ravenSession
                .Query<Employee, EmployeeByUserName_Search>()
                .Where(x => x.UserName.In(employeUserNames))
                .ToDictionary(k => k.UserName);
            return employeesProjection.Select(e =>
             {
                 return new EmployeeEvaluationDTO()
                 {
                     IsResponsible = e.ResponsibleId == _loggedUser,
                     ResponsibleId = e.ResponsibleId,
                     FullName = e.FullName,
                     UserName = e.UserName,
                     Period = e.Period,
                     CurrentPosition = employeeByUsername[e.UserName].CurrentPosition,
                     Seniority = employeeByUsername[e.UserName].Seniority,
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
                         .ToList(),
                     SharedLinks = e.SharedLinks
                 };
             }).ToList();
        }

        public string GetLastPeriodForResponisble(string username)
        {
            var evaluation = _ravenSession.
                Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Where(x => x.UserName == username && x.ResponsibleId == _loggedUser)
                .OrderByDescending(x => x.Period)
                .FirstOrDefault();
            return evaluation?.Period;
        }

        private bool getEvaluationEditable(EmployeeToEvaluate_Search.Projection projection)
        {
            if (_loggedUser != null)
            {
                if (projection.ResponsibleId == _loggedUser)
                {
                    return !projection.Finished;
                }
                return projection.CalificationsState.Any(e => e.UserName == _loggedUser && !e.Finished);
            }
            return false;
        }
    }
}

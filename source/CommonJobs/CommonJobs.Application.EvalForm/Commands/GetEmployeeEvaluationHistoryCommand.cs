using CommonJobs.Application.EvalForm.Helper;
using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEmployeeEvaluationHistoryCommand : Command<List<HistoricalEvaluationDto>>
    {
        private string _userName { get; set; }
        private string _lastPeriod { get; set; }

        public GetEmployeeEvaluationHistoryCommand(string lastPeriod, string userName)
        {
            _lastPeriod = lastPeriod;
            _userName = userName;
        }

        public override List<HistoricalEvaluationDto> ExecuteWithResult()
        {
            IQueryable<EmployeeEvaluationHistory_Search.Projection> query = RavenSession
                .Query<EmployeeEvaluationHistory_Search.Projection, EmployeeEvaluationHistory_Search>()
                .Where(e => e.UserName == _userName);

            var employeesProjection = query.ToList()
                .Where(x => x.Period != null && 0 >= string.Compare(x.Period, _lastPeriod))
                .OrderByDescending(x=>x.Period)
                .ToList();
            var mapper = new EmployeeEvaluationHelper(RavenSession, _userName);
            return employeesProjection.Select(e =>
                 new HistoricalEvaluationDto()
                 {
                     AverageCalification = e.Califications,
                     ResponsibleId = e.ResponsibleId,
                     FullName = e.FullName,
                     UserName = e.UserName,
                     Period = e.Period,
                     Evaluators = e.Evaluators != null ? e.Evaluators.ToList() : new List<string>(),
                     State = EvaluationStateHelper.GetEvaluationState(e.AutoEvaluationDone, e.ResponsibleEvaluationDone, e.CompanyEvaluationDone, e.OpenToDevolution, e.Finished),
                     Id = e.Id,
                 }).ToList();
        }
    }
}

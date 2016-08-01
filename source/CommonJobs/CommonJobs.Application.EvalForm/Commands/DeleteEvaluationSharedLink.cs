using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class DeleteEvaluationSharedLink : Command
    {
        private string _period;
        private string _userName;
        private SharedLink _sharedLink;

        public DeleteEvaluationSharedLink(string period, string userName, SharedLink sharedLink)
        {
            _period = period;
            _userName = userName;
            _sharedLink = sharedLink;
        }

        public override void Execute()
        {
            var evaluation = RavenSession.Load<EmployeeEvaluation>(EmployeeEvaluation.GenerateEvaluationId(_period, _userName));
            if (evaluation == null)
            {
                throw new ApplicationException($"Could not find evaluation for period {_period} and user name {_userName}");
            }
            if (evaluation.SharedLinks == null || evaluation.SharedLinks.RemoveAll(x=>x.SharedCode==_sharedLink.SharedCode)<1)
            {
                throw new ApplicationException($"This evaluation does not contain a Shared link with code {_sharedLink.SharedCode}. Unable to delete");
            }
        }
    }
}

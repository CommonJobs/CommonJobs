using CommonJobs.Application.EvalForm.Helper;
using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class CreateEvaluationSharedLink : Command<SharedLink>
    {
        private string _period;
        private string _userName;

        public CreateEvaluationSharedLink(string period, string userName)
        {
            _period = period;
            _userName = userName;
        }

        public override SharedLink ExecuteWithResult()
        {
            var evaluation = RavenSession.Load<EmployeeEvaluation>(EmployeeEvaluation.GenerateEvaluationId(_period, _userName));
            if (evaluation == null)
            {
                throw new ApplicationException($"Could not find evaluation for period {_period} and user name {_userName}");
            }
            if (evaluation.SharedLinks == null)
            {
                evaluation.SharedLinks = new SharedLinkList();
            }
            //regular expression for strings that starts with "Link#" and followed only by numbers
            var regExp = new Regex(@"^Link#(\d+)$");
            var lastLinkNumber = evaluation.SharedLinks
            .Select(x => x.FriendlyName)
            .Select(x=>regExp.Match(x))
            .Where(x => x.Success)
            .Select(x =>int.Parse(x.Groups[1].Value))
            .OrderBy(x => x)
            .LastOrDefault();

            var newSharedLink = new SharedLink()
            {
                FriendlyName = $"Link#{lastLinkNumber + 1}",
                ExpirationDate = DateTime.Now.AddDays(3).ToUniversalTime(),
                SharedCode = GenerateSharedCode()
            };
            evaluation.SharedLinks.Add(newSharedLink);
            RavenSession.SaveChanges();
            return newSharedLink;
        }

        private string GenerateSharedCode()
        {
            var random = new Random();
            var randomstring = "";
            var codeLenght = 12;
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
            for (var i = 0; i < codeLenght; i++)
            {
                var rnum = int.Parse(Math.Floor(random.NextDouble() * chars.Length).ToString());
                randomstring += chars.Substring(rnum, 1);
            }
            return randomstring;
        }
    }
}

using CommonJobs.Application.Evaluations;
using CommonJobs.Domain;
using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Mvc.UI.Infrastructure;
using NLog;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Utilities;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Mvc.UI.Areas.Evaluations.Models;
using CommonJobs.Application.Evaluations;
using CommonJobs.Application.EvalForm.Commands;

namespace CommonJobs.Mvc.UI.Areas.Evaluations
{
    [CommonJobsAuthorize]
    //TODO: Add documentation
    [Documentation("docs/manual-de-usuario/evaluaciones")]
    public class EvaluationsController : CommonJobsController
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        private string DetectUser()
        {
            //TODO: remove hardcoded "CS\\"
            //TODO: move to an AuthorizeAttribute or something more elegant
            if (User != null && User.Identity != null && User.Identity.Name != null)
            {
                var user = User.Identity.Name;
                if (user.StartsWith("CS\\"))
                    user = user.Substring(3);
                return user;
            }
            else
            {
                throw new ApplicationException("User cannot be detected");
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PeriodCreation(string period){
            ViewBag.Period = period;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PeriodEvaluation(string period)
        {
            //1. Check if period exists by querying Evaluations collection.
            //2. Check if the current user has to evaluate someone
            //2.a. If not, redirect to /Evaluations/{period}/{username}
            //2.b. If so, fetch the users to evaluate

            /// GK TESTING, PLEASE DELETE
            //var employees = ExecuteCommand(new GetEmployeesForEvaluationCommand("2015-06"));
            //ExecuteCommand(new GenerateEvaluationsCommand(employees));

            //ExecuteCommand(new AddEvaluatorsCommand(employees[0], new List<string>() { "Users/gkolocsar" }));

            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Calification(string period, string username)
        {
            //1. If user is self
            //1.a. Check if it needs to be evaluated (if not, redirect or error)
            //1.b. If auto-evaluation is not done, show auto-evaluation to complete
            //1.c. If auto-evaluation is done, show auto-evaluation (no edit)
            //1.d. If auto-evaluation is done and company is done, show both

            //2. If not self
            //2.a. Check if it's responsible or evaluator of the user
            //2.a.1. If not, redirect or error
            //2.a.2. If it is, show the next point with the rest of evaluations
            //2.b. Evaluator/Responsible with no evaluation done, show standard evaluation to complete
            //2.c. Evaluator with evaluation done, show evaluation done
            //2.d. Responsible with evaluation done, show company evaluation to complete
            //2.e. Responsible with company evaluation done, show every evaluation including auto
            ViewBag.Period = period;
            ViewBag.UserName = username;
            ViewBag.IsCalification = true;
            return View("Calification");
        }
    }
}

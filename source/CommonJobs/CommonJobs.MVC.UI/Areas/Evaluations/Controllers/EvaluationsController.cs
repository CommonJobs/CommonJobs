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
using CommonJobs.Application.EvalForm.EmployeeSearching;
using System.Web.Routing;

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
        [CommonJobsAuthorize(Roles = "EmployeeManagers")]
        public ActionResult PeriodCreation(string period){
            ViewBag.Period = period;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Documentation("docs/manual-de-usuario/evaluaciones#Herramienta_para_Responsables_y_Calificadores")]
        public ActionResult PeriodEvaluation(string period)
        {
            //1. Check if period exists by querying Evaluations collection.
            //2. Check if the current user has to evaluate someone
            //2.a. If not, redirect to /Evaluations/{period}/{username}
            //2.b. If so, fetch the users to evaluate

            var loggedUser = DetectUser();

            RavenQueryStatistics stats;
            IQueryable<EmployeeToEvaluate_Search.Projection> query = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => e.Period == period && (e.UserName == loggedUser || e.ResponsibleId == loggedUser || (e.Evaluators != null && e.Evaluators.Any(x => x == loggedUser))))
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            var evaluation = query.ToList();

            if (evaluation.Count == 0)
            {
                return new HttpStatusCodeResult(403, "Access Denied");
            }

            var isEvaluated = evaluation.Any(p => p.UserName == loggedUser);
            var isEvaluator = evaluation.Any(p => p.ResponsibleId == loggedUser || (p.Evaluators != null && p.Evaluators.Contains(loggedUser)));

            if (!isEvaluator)
            {
                if (isEvaluated)
                {
                    return RedirectToAction("Calification", new RouteValueDictionary( 
                        new { controller = "Evaluations", action = "Calification", period = period, username = loggedUser } ));
                }
            }
            ViewBag.Period = period;
            ViewBag.hasAutoCalification = isEvaluated;
            ViewBag.IsDashboard = true;
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

            //var evId = EmployeeEvaluation.GenerateEvaluationId(period, username);
            var loggedUser = DetectUser();

            RavenQueryStatistics stats;
            EmployeeToEvaluate_Search.Projection evaluation = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => e.UserName == username && e.Period == period)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite()).FirstOrDefault();

            // The evaluation doesn't exist
            if (evaluation == null)
            {
                return HttpNotFound();
            }

            // If it's not the user, it must validate that it's responsible or evaluator
            if (loggedUser != username)
            {
                if (evaluation.ResponsibleId != loggedUser)
                {
                    if (!(evaluation.Evaluators != null && evaluation.Evaluators.Contains(loggedUser)))
                    {
                        return new HttpStatusCodeResult(403, "Access Denied");
                    }
                }
                ViewBag.IsUserEvaluator = true;
            }         

            ViewBag.Period = period;
            ViewBag.UserName = username;
            ViewBag.IsCalification = true;
            return View("Calification");
        }
    }
}

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
using CommonJobs.Application.EvalForm.Indexes;
using System.Web.Routing;
using Raven.Client;

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
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated && User.Identity.Name != null)
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
        public ActionResult PeriodCreation(string period)
        {
            ViewBag.Period = period;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [CommonJobsAuthorize(Roles = "EmployeeManagers,EvaluationManagers")]
        public ActionResult ReportDashboard(string period)
        {
            var selectList = GetReportPeriods().Select(x => x.Period).Distinct().Select(x => new SelectListItem
            {
                Text = x,
                Value = Url.Action(period),
                Selected = x == period
            });

            ViewBag.Period = period;
            ViewBag.IsReportDashboard = true;
            ViewBag.ReportPeriods = selectList;

            ScriptManager.RegisterGlobalJavascript(
               "ViewData",
               new
               {
                   period = period,
                   isEvaluationManager = IsEvaluationManager(DetectUser())
               },
               500);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [CommonJobsAuthorize(Roles="EmployeeManagers,EvaluationManagers")]
        public ActionResult ReportDashboardIndex()
        {
            var lastPeriod = GetReportPeriods().Select(e => e.Period).FirstOrDefault();

            return RedirectToAction("ReportDashboard", "Evaluations", new { period = lastPeriod });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Documentation("docs/manual-de-usuario/evaluaciones#Herramienta_para_Responsables_y_Calificadores")]
        public ActionResult PeriodEvaluation(string period)
        {
            //1. Check if period exists by querying Evaluations collection.
            //2. Check if the current user has to evaluate someone

            var loggedUser = DetectUser();

            RavenQueryStatistics stats;
            IQueryable<EmployeeToEvaluate_Search.Projection> query = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => e.Period == period && (e.UserName == loggedUser || e.ResponsibleId == loggedUser || (e.Evaluators != null && e.Evaluators.Any(x => x == loggedUser))));

            var evaluation = query.ToList();

            if (evaluation.Count == 0)
            {
                return new HttpStatusCodeResult(403, "Access Denied");
            }

            var selectList = GetPeriods().Select(x => new SelectListItem
            {
                Text = x.Period,
                Value = Url.Action(period),
                Selected = x.Period == period
            });
            ViewBag.UserPeriods = selectList;

            var isEvaluated = evaluation.Any(p => p.UserName == loggedUser);
            ViewBag.Period = period;
            ViewBag.hasAutoCalification = isEvaluated;
            ViewBag.isAutoEvaluationOpenForDevolution = evaluation.Any(e => e.UserName == loggedUser && e.OpenToDevolution);
            ViewBag.IsDashboard = true;

            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    period = period,
                },
                500);
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

            var loggedUser = DetectUser();

            RavenQueryStatistics stats;
            EmployeeToEvaluate_Search.Projection evaluation = RavenSession
                .Query<EmployeeToEvaluate_Search.Projection, EmployeeToEvaluate_Search>()
                .Statistics(out stats)
                .Where(e => e.UserName == username && e.Period == period).FirstOrDefault();

            // The evaluation doesn't exist
            if (evaluation == null)
            {
                return HttpNotFound();
            }

            var isManager = IsEmployeeManager(loggedUser);
            var isLoggedUserEvaluator = IsEvaluator(loggedUser, username, evaluation.ResponsibleId, evaluation.Evaluators);
            var isLoggedUserEvaluated = loggedUser == username;

            if (!isLoggedUserEvaluated && !isLoggedUserEvaluator && !isManager)
            {
                return new HttpStatusCodeResult(403, "Access Denied");
            }

            ViewBag.IsUserEvaluator = isLoggedUserEvaluator;

            ViewBag.Period = period;

            ViewBag.UserName = username;
            ViewBag.IsCalification = true;
            return View("Calification");
        }

        public ActionResult Index()
        {
            var userLastPeriod = GetPeriods().Select(e => e.Period).FirstOrDefault();

            return RedirectToAction("PeriodEvaluation", "Evaluations", new { period = userLastPeriod });
        }

        private bool IsEmployeeManager(string username)
        {
            return CheckRole(username, "EmployeeManagers");
        }

        private bool IsEvaluationManager(string username)
        {
            return CheckRole(username, "EvaluationManagers");
        }

        private bool CheckRole(string username, params string [] role)
        {
            var sessionRoles = ExecuteCommand(new GetLoggedUserRoles(username));
            return sessionRoles.Intersect(role).Any();
        }

        private bool IsEvaluator(string loggedUser, string evaluatedUser, string responsibleId, string[] evaluators)
        {
            return responsibleId == loggedUser || (evaluators != null && evaluators.Contains(loggedUser));
        }

        private List<Period_Search.Projection> GetPeriods()
        {
            return RavenSession
                  .Query<Period_Search.Projection, Period_Search>()
                  .Where(e => (e.UserName == DetectUser()))
                  .OrderByDescending(e => e.Period)
                  .ToList();
        }

        private List<Period_Search.Projection> GetReportPeriods()
        {
            return RavenSession
                .Query<Period_Search.Projection, Period_Search>()
                .OrderByDescending(e => e.Period)
                .ToList();
        }
    }
}

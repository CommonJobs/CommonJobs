using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.Evaluations
{
    public class EvaluationsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Evaluations";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Evaluations_default",
                "Evaluations/api/{action}/{period}/{username}/",
                new { controller = "EvaluationsApi", period = UrlParameter.Optional, username = UrlParameter.Optional }
            );

            context.MapRoute(
                "Evaluations_report",
                "Evaluations/report",
                new { controller = "Evaluations", action = "ReportDashboard" }
            );

            context.MapRoute(
                "Evaluations_creation",
                "Evaluations/create/{period}",
                new { controller = "Evaluations", action = "PeriodCreation", period = UrlParameter.Optional}
            );

            context.MapRoute(
                "Evaluations_index",
                "Evaluations/Index",
                new { controller = "Evaluations", action = "Index" }
                );

            context.MapRoute(
                "Evaluations_period",
                "Evaluations/{period}",
                new { controller = "Evaluations", action = "PeriodEvaluation" }
            );

            context.MapRoute(
                "Evaluations_califications",
                "Evaluations/{period}/{username}",
                new { controller = "Evaluations", action = "Calification" }
            );
        }
    }
}

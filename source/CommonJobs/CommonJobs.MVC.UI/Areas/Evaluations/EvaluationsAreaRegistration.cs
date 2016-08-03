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
                "Evaluations_api_default_shared",
                "Evaluations/api/{action}/{period}/{username}/{sharedCode}",
                new { controller = "EvaluationsApi", period = UrlParameter.Optional, username = UrlParameter.Optional, sharedCode = UrlParameter.Optional }
            );

            context.MapRoute(
                "Evaluations_api_default",
                "Evaluations/api/{action}/{period}/{username}/",
                new { controller = "EvaluationsApi", period = UrlParameter.Optional, username = UrlParameter.Optional }
            );

            context.MapRoute(
            "Evaluations_report_index",
            "Evaluations/report",
            new { controller = "Evaluations", action = "ReportDashboardIndex" }
             );

            context.MapRoute(
                "Evaluations_report",
                "Evaluations/report/{period}",
                new { controller = "Evaluations", action = "ReportDashboard", period = UrlParameter.Optional }
            );

            context.MapRoute(
                "Evaluations_history",
                "Evaluations/history/{username}",
                new { controller = "Evaluations", action = "HistoryDashboard", username = UrlParameter.Optional }
            );

            context.MapRoute(
                "Evaluations_creation",
                "Evaluations/create/{period}",
                new { controller = "Evaluations", action = "PeriodCreation", period = UrlParameter.Optional }
            );

            context.MapRoute(
                "Evaluations_default",
                "Evaluations",
                new { controller = "Evaluations", action = "Index" }
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
               "Evaluations_califications_shared",
               "Evaluations/{period}/{username}/shared/{sharedcode}",
               new { controller = "Evaluations", action = "Calification", sharedcode = UrlParameter.Optional }
           );

            context.MapRoute(
                "Evaluations_califications",
                "Evaluations/{period}/{username}",
                new { controller = "Evaluations", action = "Calification" }
            );
        }
    }
}

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
                "Evaluations_period",
                "Evaluations/{period}",
                new { controller = "Evaluations", action = "PeriodEvaluation" }
            );

            context.MapRoute(
                "Evaluations_califications",
                "Evaluations/{period}/{username}",
                new { controller = "Evaluations", action = "Calification" }
            );

            // If we need some AJAX call...
            //context.MapRoute(
            //    "Evaluations_default",
            //    "Evaluations/api/{action}/{id}",
            //    new { controller = "Evaluations", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}

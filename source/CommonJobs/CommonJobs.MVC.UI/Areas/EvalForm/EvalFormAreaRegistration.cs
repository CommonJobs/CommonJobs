using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.EvalForm
{
    public class EvalFormAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EvalForm";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "EvalForm_default",
                "EvalForm/{action}/{id}",
                new { controller = "EvalForm", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

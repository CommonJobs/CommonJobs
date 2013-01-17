using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.Profiles
{
    public class ProfilesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Profiles";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Profiles_default",
                "Profiles/{controller}/{action}/{id}",
                new { controller = "Profiles", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.MyMenu
{
    public class MyMenuAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MyMenu";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyMenu_default",
                "MyMenu/{action}/{id}",
                new { controller = "MyMenu", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

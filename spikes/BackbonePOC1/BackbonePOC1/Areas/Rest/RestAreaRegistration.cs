using System.Web.Mvc;

namespace BackbonePOC1.Areas.Rest
{
    public class RestAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Rest";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Rest_Get",
                "rest/categories",
                new { controller = "Categories", action = "Get" }
            );
        }
    }
}

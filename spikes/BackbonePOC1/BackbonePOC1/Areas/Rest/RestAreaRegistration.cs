using System.Web.Mvc;
using System.Web.Routing;

namespace BackbonePOC1.Areas.Rest
{
    public class RestAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "rest"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Rest_Get",
                "rest/{controller}",
                new { action = "Get" },
                new { httpMethod = new HttpMethodConstraint("GET") }
            );
            context.MapRoute(
                "Rest_Post",
                "rest/{controller}",
                new { action = "Post" },
                new { httpMethod = new HttpMethodConstraint("POST") }
            );
        }
    }
}

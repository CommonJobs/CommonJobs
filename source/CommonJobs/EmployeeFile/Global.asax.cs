using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CommonJobs.Mvc;
using CommonJobs.Infrastructure.Indexes;

namespace EmployeeFile
{
    public class MvcApplication : CommonJobsApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{*id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected override System.Reflection.Assembly[] GetIndexAssemblies()
        {
            return new[] { typeof(NullIndex).Assembly };
        }
        
        protected void Application_Start()
        {
            Initialize();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);
        }
    }
}
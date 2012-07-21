using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CommonJobs.ContentExtraction;
using CommonJobs.ContentExtraction.Extractors;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.Raven.Mvc;

namespace CommonJobs.Mvc.PublicUI
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
            //TODO: move it to a common place in order to have the same configuration in CommonJobs.Mvc.UI and CommonJobs.Mvc.PublicUI
            ContentExtractionConfiguration.Current.Clear();
            ContentExtractionConfiguration.Current.AddRange(new IContentExtractor[] {
                new PlainTextContentExtractor(), 
                new FilterContentExtractor()
            });

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);
        }
    }
}
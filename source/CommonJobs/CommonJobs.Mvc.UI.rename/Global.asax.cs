using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CommonJobs.Raven.Mvc;
using CommonJobs.Infrastructure.Indexes;
using CommonJobs.ContentExtraction;
using CommonJobs.ContentExtraction.Extractors;

namespace CommonJobs.Mvc.UI
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
            using (var migrator = new DataMigrator(RavenSessionManager.DocumentStore))
                migrator.Execute();
            
            //TODO: hacer esto con algo mejor que un singleton
            ContentExtractionConfiguration.Current.Clear();
            ContentExtractionConfiguration.Current.AddRange(new IContentExtractor[] {
                new PlainTextContentExtractor(), //I prefeer my own plain text extractor than IFilter. It is ugly but it reads fine UTF8 without BOM files.
                new FilterContentExtractor()
            });

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);
        }
    }
}
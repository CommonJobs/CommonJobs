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
            //TODO: acá se podrían invocar las migraciones automáticas
            
            //TODO: hacer esto con algo mejor que un singleton
            ContentExtractionConfiguration.Current.Clear();
            ContentExtractionConfiguration.Current.AddRange(new IContentExtractor[] {
                new PlainTextContentExtractor(), //I prefeer my own plain text extractor than IFilter. It is ugly but it reads fine UTF8 without BOM files.
                new FilterContentExtractor()
            });

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);

            RegisterRoutes(RouteTable.Routes);

            //TODO: Hacer depender esto del environment, por ejemplo un grupo en DEV podría ser CommonJobsDEV_Users y en PROD CommonJobs_Users
            CommonJobsAuthorizeAttribute.SetPrefixMapping("CommonJobsDEV_");

            //TODO: Solo en DEV local no en RABBITMQ, para saltarnos AD
            CommonJobsAuthorizeAttribute.SetFakeRolesFromSetting("FakeGroups");

            //NOTA: en los unit tests se podría hacer algo así:
            //CommonJobsAuthorizeAttribute.SetFakeRolesFromString("Users, Migrators");
            //Assert(MigrationsWorks);
            //CommonJobsAuthorizeAttribute.SetFakeRolesFromString("Users");
            //Assert(MigrationsNotWorks);
        }
    }
}
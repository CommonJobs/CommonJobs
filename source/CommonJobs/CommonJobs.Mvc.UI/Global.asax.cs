using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Application.Indexes;
using CommonJobs.ContentExtraction;
using CommonJobs.ContentExtraction.Extractors;
using CommonJobs.Infrastructure.Mvc.Authorize;
using Raven.Client.Listeners;
using CommonJobs.Infrastructure.RavenDb.Schedule;
using NLog;

namespace CommonJobs.Mvc.UI
{
    public class MvcApplication : CommonJobsApplication
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Shared", // Route name
                "{controller}/{action}/shared/{sharedCode}/{*id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

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

        protected override IEnumerable<IDocumentStoreListener> GetDocumentStoreListeners()
        {
            yield return new SetLastModifiedByMetadataListener();
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

#if NO_AD
            CommonJobsAuthorizeAttribute.AuthorizationBehavior = new ForcedGroupsFromSettingsAuthorizationBehavior("CommonJobs/FakeADGroups");
#else
            CommonJobsAuthorizeAttribute.AuthorizationBehavior = new PrefixFromSettingsAuthorizationBehavior("CommonJobs/ADGroupsPrefix");
#endif

            StartTimer();
        }

        private System.Threading.Timer timer;
        private void StartTimer()
        {
            bool working = false;
            timer = new System.Threading.Timer((state) =>
            {
                //Block is not necessary because it is called periodically
                if (!working)
                {
                    working = true;
                    try
                    {
                        using (var session = RavenSessionManager.DocumentStore.OpenSession())
                        {
                            (new ExecuteScheduledTasks() { RavenSession = session }).Execute();
                        }
                    }
                    catch (Exception e)
                    {
                        log.ErrorException("Error on global.asax timer", e);
                    }
                    working = false;
                }
            }, 
            null, 
            TimeSpan.FromMinutes(2),
            //TODO: make period configurable
            TimeSpan.FromMinutes(4));
        }
    }
}
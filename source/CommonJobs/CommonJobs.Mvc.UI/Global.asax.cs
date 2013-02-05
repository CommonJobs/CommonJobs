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


            CommonJobsAuthorizeAttribute.AuthorizationBehavior = new MixedAuthorizationBehavior(
                new SessionAndExternalRolesAuthorizationBehavior(CommonJobs.Mvc.UI.Controllers.AccountController.SessionRolesKey, userName =>
                {
                    using (var session = RavenSessionManager.DocumentStore.OpenSession())
                    {
                        var user = session.Query<CommonJobs.Domain.User>().Where(u => u.UserName == userName).FirstOrDefault();
                        return user.Roles ?? new string[0];
                    }
                }),
                new PrefixFromSettingsAuthorizationBehavior("CommonJobs/ADGroupsPrefix"));

            // Es cierto que iniciar recurrentes aquí puede no ser una buena idea (http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx)
            // Pero es la mejor forma de lograr un deploy simple, y a la vez soporar AppHarbor.
            // Los problemas indicados por Phil Haack no deberían causar inconvenientes en esta aplicación
            // Hay un problema: la aplicación no se inicia sola
            //TODO: make period configurable
            ExecuteScheduledTasks.StartPeriodicTasks(RavenSessionManager.DocumentStore);
        }
    }
}
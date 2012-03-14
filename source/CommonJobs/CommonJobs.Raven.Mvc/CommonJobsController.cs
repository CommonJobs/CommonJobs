using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Raven.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using CommonJobs.Raven.Infrastructure;

namespace CommonJobs.Raven.Mvc
{
    public abstract class CommonJobsController : Controller
    {
        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = RavenSessionManager.GetCurrentSession();
        }

        public ScriptManager ScriptManager
        {
            get { return ScriptManager.GetFromViewData(ViewData); }
        }

        protected new JsonNetResult Json(object data)
        {
            return new JsonNetResult(data, GetSerializerSettings());
        }

        //TODO: refactor it, or move to a better place
        public static JsonSerializerSettings GetSerializerSettings()
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new[] 
                { 
                    //http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "s" } 
                }
            };
            return settings;
        }

        //TODO: Tal vez sea util en los tests: http://ayende.com/blog/154273/limit-your-abstractions-and-how-do-you-handle-testing
        protected void ExecuteCommand(Command cmd)
        {
            cmd.RavenSession = RavenSession;
            cmd.Execute();
        }

        protected TResult ExecuteCommand<TResult>(Command<TResult> cmd)
        {
            cmd.RavenSession = RavenSession;
            cmd.Execute();
            return cmd.Result;
        }

        protected TResult Query<TResult>(Query<TResult> qry)
        {
            qry.RavenSession = RavenSession;
            return qry.Execute();
        }
    }
}
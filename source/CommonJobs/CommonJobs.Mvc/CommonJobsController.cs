using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Raven.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CommonJobs.Mvc
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
            var settings = new JsonSerializerSettings();
            return new JsonNetResult(data, settings);
        }
    }
}
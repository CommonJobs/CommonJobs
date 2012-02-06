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
            return new JsonNetResult(data, GetSerializerSettings());
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            //TODO: verificar que este método y HtmlScriptManagerExtensions.RenderGlobalJavascript 
            //generen los mismos objetos javascript del lado del cliente (verificar fechas).
            //No importa realmente como lo serializa, ya que HtmlScriptManagerExtensions.RenderGlobalJavascript 
            //es javascript puro, encambio el resultado de este método es deserializado por json2.
            //En caso de que las fechas no se generen correctamente jugar con las JsonSerializerSettings de este
            //método y con JSON.parse(text, reviver) como se explica en json2.js
            var converter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
            converter.DateTimeFormat = "s"; //http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
            //TODO: add something like that in the client:
            //JSON.parse(jsonstring, function (key, value) {
            //    var a;
            //    if (typeof value === 'string') {
            //        a = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)$/.exec(value);
            //        if (a) {
            //            return new Date(Date.UTC(+a[1], +a[2] - 1, +a[3], +a[4], +a[5], +a[6]));
            //        }
            //    }
            //    return value;
            //}
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new[] { converter }
            };
            return settings;
        }
    }
}
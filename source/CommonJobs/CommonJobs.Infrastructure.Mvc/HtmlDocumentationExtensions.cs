using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using CommonJobs.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.Web.WebPages;

namespace CommonJobs.Infrastructure.Mvc
{
    public static class HtmlDocumentationExtensions
    {
        public static string BaseUrl 
        { 
            get { return ConfigurationManager.AppSettings["CommonJobs/DocumentationBaseUrl"].AppendIfDoesNotEndWith("/"); }
        }

        public static HelperResult DocumentationLink<T>(this HtmlHelper<T> htmlHelper, string label)
        {
            DocumentationAttribute attribute = null;
            var viewContext = htmlHelper.ViewContext;
            
            var controllerDescriptor = new ReflectedControllerDescriptor(viewContext.Controller.GetType());

            var actionName = viewContext.RouteData.Values["action"] as string;
            var actionDescriptor = controllerDescriptor
                .GetCanonicalActions()
                .Where(x => String.Equals(x.ActionName, actionName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            if (actionDescriptor != null)
            {
                attribute = actionDescriptor
                    .GetCustomAttributes(typeof(DocumentationAttribute), true)
                    .Cast<DocumentationAttribute>()
                    .FirstOrDefault();
            }

            if (attribute == null)
            {
                attribute = controllerDescriptor
                    .GetCustomAttributes(typeof(DocumentationAttribute), true)
                    .Cast<DocumentationAttribute>()
                    .FirstOrDefault();
            }

            var link = attribute == null 
                ? BaseUrl
                : string.Format("{0}{1}", BaseUrl, attribute.DocumentPath);

            return new HelperResult(writer =>
            {
                var builder = new TagBuilder("a");
                builder.MergeAttribute("href", link);
                builder.MergeAttribute("target", "_blank");
                writer.Write(builder.ToString(TagRenderMode.StartTag));
                writer.Write(label);
                writer.Write(builder.ToString(TagRenderMode.EndTag));
            });
        }
    }
}
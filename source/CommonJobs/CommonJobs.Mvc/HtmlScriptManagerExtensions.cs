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

namespace CommonJobs.Mvc
{
    public static class HtmlScriptManagerExtensions
    {
        public static MvcHtmlString RenderScriptManagerEntries(this HtmlHelper htmlHelper)
        {
            var scriptManager = ScriptManager.GetFromViewData(htmlHelper.ViewData);
            var entries = scriptManager.GetEntries();

            var sb = new StringBuilder();
            sb.AppendLine().AppendLine("<!-- Begin ScriptManager Entries -->");
            foreach (var entry in entries)
            {
                var renderedEntry = FunctionHelper.FirtsNotNull(entry, RenderCssReference, RenderJsReference, RenderGlobalJavascript);
                if (renderedEntry != null)
                    sb.AppendLine(renderedEntry);
            }
            sb.AppendLine("<!-- End ScriptManager Entries -->");
            return MvcHtmlString.Create(sb.ToString());
        }

        private static string RenderCssReference(ScriptManagerEntry entry)
        {
            var casted = entry as CssReferenceEntry;
            if (casted == null)
                return null;

            var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(casted.HtmlAttributes);
            TagBuilder builder = new TagBuilder("link");
            builder.MergeAttribute("href", casted.Path);
            builder.MergeAttribute("rel", "stylesheet");
            builder.MergeAttribute("type", "text/css");
            builder.MergeAttributes(htmlAttributes);
            return builder.ToString(TagRenderMode.SelfClosing);
        }

        private static string RenderJsReference(ScriptManagerEntry entry)
        {
            var casted = entry as JsReferenceEntry;
            if (casted == null)
                return null;
            
            var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(casted.HtmlAttributes);
            TagBuilder builder = new TagBuilder("script");
            builder.MergeAttribute("src", casted.Path);
            builder.MergeAttribute("type", "text/javascript");
            builder.MergeAttributes(htmlAttributes);
            return builder.ToString(TagRenderMode.Normal);
        }

        private static string RenderGlobalJavascript(ScriptManagerEntry entry)
        {
            var casted = entry as GlobalJavascriptEntry;
            if (casted == null)
                return null;

            var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(casted.HtmlAttributes);
            TagBuilder builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");
            builder.MergeAttributes(htmlAttributes);
            builder.SetInnerText(string.Format("window.{0} = {1};", 
                casted.Name, 
                JsonConvert.SerializeObject(
                    casted.Value,
                    Formatting.Indented,
                    GetSerializerSettings())));
            return builder.ToString(TagRenderMode.Normal);
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var converter = new JavaScriptDateTimeConverter();
            return new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new[] { converter }
            };
        }
    }
}
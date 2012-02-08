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
        private const string APP_VERSION_QUERYSTRING_KEY = "appv";
        /// <summary>
        /// Render references and scripts entries in header
        /// </summary>
        public static MvcHtmlString RenderScriptManagerEntries(this HtmlHelper htmlHelper)
        {
            var scriptManager = ScriptManager.GetFromViewData(htmlHelper.ViewData);
            var entries = scriptManager.GetEntries();

            var sb = new StringBuilder().AppendLine();
            foreach (var entry in entries)
            {
                var renderedEntry = FunctionHelper.FirtsNotNull(
                    () => RenderReference(entry),
                    () => RenderGlobalJavascript(entry));
                if (renderedEntry != null)
                    sb.AppendLine(renderedEntry);
            }
            return MvcHtmlString.Create(sb.ToString());
        }

        private static string RenderReference(ScriptManagerEntry entry)
        {
            var casted = entry as ReferenceEntry;
            if (casted == null)
                return null;

            TagBuilder builder = new TagBuilder(casted.TagName());
            builder.MergeAttribute(casted.ReferenceAttributeName(), TransformReferencePath(casted.Path, casted.OmitAppVersion));
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(casted.DefaultAttributes()));
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(casted.HtmlAttributes));
            return builder.ToString(casted.SelfClosed() ? TagRenderMode.SelfClosing : TagRenderMode.Normal);
        }

        private static string RenderGlobalJavascript(ScriptManagerEntry entry)
        {
            var casted = entry as GlobalJavascriptEntry;
            if (casted == null)
                return null;

            TagBuilder builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(casted.HtmlAttributes));

            builder.SetInnerText(string.Format("window.{0} = {1};", 
                casted.Name, 
                JsonConvert.SerializeObject(
                    casted.Value,
                    Formatting.Indented,
                    GetSerializerSettings())));
            return builder.ToString(TagRenderMode.Normal);
        }

        /// <summary>
        /// It will allow us to use aggresive catching of referenced files without pain
        /// </summary>
        private static string TransformReferencePath(string path, bool omitAppVersion)
        {
            if (omitAppVersion)
                return path;

            var separator = path.Contains('?') ? "&" : "?";
            return string.Format("{0}{1}{2}={3}", path, separator, APP_VERSION_QUERYSTRING_KEY, CommonJobsApplication.AppNameHash);
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
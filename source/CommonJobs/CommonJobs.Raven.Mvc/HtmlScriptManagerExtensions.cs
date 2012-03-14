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

namespace CommonJobs.Raven.Mvc
{
    public static class HtmlScriptManagerExtensions
    {
        private const string APP_VERSION_QUERYSTRING_KEY = "appv";
        /// <summary>
        /// Render references and scripts entries in header
        /// </summary>
        public static MvcHtmlString RenderScriptManagerEntries<T>(this HtmlHelper<T> htmlHelper)
        {
            var scriptManager = ScriptManager.GetFromViewData(htmlHelper.ViewData);
            var entries = scriptManager.GetEntries();

            var sb = new StringBuilder().AppendLine();
            
            foreach (var entry in entries)
            {
                var renderedEntry = FunctionHelper.FirtsNotNull(
                    () => RenderReference(entry),
                    () => RenderGlobalJavascript(entry),
                    () => RenderGlobalizationEntries(entry));
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
            var tag = builder.ToString(casted.SelfClosed() ? TagRenderMode.SelfClosing : TagRenderMode.Normal);
            return ProcessPatchCondition(casted, tag);
        }

        private static string ProcessPatchCondition(ReferenceEntry entry, string tag)
        {
            var casted = entry as CssReferenceEntry;
            if (casted != null && casted.PatchCondition != null)
                tag = string.Format("<!--[if {0}]>{1}<![endif]-->", casted.PatchCondition, tag);
            return tag;   
        }

        //TODO: refactorize it
        private static string RenderGlobalizationEntries(ScriptManagerEntry entry)
        {
            var casted = entry as GlobalizationEntries;
            if (casted == null)
                return null;

            var meta = String.Format("<meta name=\"accept-language\" content=\"{0}\">", casted.AcceptLanguage);

            var files = new List<string>() {
                "globalize.js",
                string.Format("cultures/globalize.culture.{0}.js", casted.AcceptLanguage)
            };
            if (casted.AcceptLanguage != casted.AcceptLanguageSimplied)
                files.Add(string.Format("cultures/globalize.culture.{0}.js", casted.AcceptLanguageSimplied));
            var references = files.Select(x => HtmlScriptManagerExtensions.RenderReference(new JsReferenceEntry() { Path = string.Format("{0}/{1}", casted.GlobalizeScriptFolder, x) }));
            var scriptTemplates = new[] {
                @"if (Globalize.cultures['{0}']) {{
    Globalize.culture('{0}'); 
}} else if (Globalize.cultures['{1}']) {{
    Globalize.culture('{1}'); 
}}",
                @"if ($) {{$(function () {{
    if ($.datepicker) {{
        $.datepicker.setDefaults($.datepicker.regional['{0}'] || $.datepicker.regional['{1}'] || $.datepicker.regional['']);
    }}
}});}}"};
            var scripts = scriptTemplates.Select(x => string.Format(x, casted.AcceptLanguage, casted.AcceptLanguageSimplied));
            TagBuilder builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");

            return string.Join("\n", 
                new[] { meta } 
                .Union(references)
                .Union(new[] { builder.ToString(TagRenderMode.StartTag) })
                .Union(scripts)
                .Union(new[] { builder.ToString(TagRenderMode.EndTag) }));
        }

        private static string RenderGlobalJavascript(ScriptManagerEntry entry)
        {
            var casted = entry as GlobalJavascriptEntry;
            if (casted == null)
                return null;

            TagBuilder builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");
            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(casted.HtmlAttributes));

            return string.Format("{0}window.{1} = {2};{3}",
                builder.ToString(TagRenderMode.StartTag),
                casted.Name,
                JsonConvert.SerializeObject(
                    casted.Value,
                    Formatting.Indented,
                    CommonJobsController.GetSerializerSettings()),
                builder.ToString(TagRenderMode.EndTag));
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
    }
}
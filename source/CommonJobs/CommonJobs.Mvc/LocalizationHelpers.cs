using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc
{
    //TODO: refactor this class
    public static class LocalizationHelpers
    {
        private static string AcceptLanguage
        {
            get { return HttpUtility.HtmlAttributeEncode(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());}
        }

        public static IHtmlString MetaAcceptLanguage<T>(this HtmlHelper<T> html)
        {
            return new HtmlString(String.Format("<meta name=\"accept-language\" content=\"{0}\">", AcceptLanguage));
        }

        public static IHtmlString LocalizationScripts<T>(this HtmlHelper<T> html, UrlHelper url)
        {
            var files = new[] {
                url.Content("~/Scripts/jquery.globalize/globalize.js"),
                url.Content(string.Format("~/Scripts/jquery.globalize/cultures/globalize.culture.{0}.js", AcceptLanguage))
            };

            var references = files.Select(x => HtmlScriptManagerExtensions.RenderReference(new JsReferenceEntry() { Path = x }));

            TagBuilder builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");

            var scripts = new[] { 
                builder.ToString(TagRenderMode.StartTag),
                string.Format("Globalize.culture('{0}')\n", AcceptLanguage),
                builder.ToString(TagRenderMode.EndTag) };

            return new HtmlString(string.Join("\n", references.Union(scripts)));
        }
    }
}

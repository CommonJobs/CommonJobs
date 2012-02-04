using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace CommonJobs.Mvc
{
    public static class HtmlScriptManagerExtensions
    {
        private static TResult FirtsNotNull<T, TResult>(T param, params Func<T, TResult>[] funcs) where TResult : class
        {
            TResult r = null;
            foreach (var f in funcs)
            {
                r = f(param);
                if (r != null)
                    break;
            }
            return r;
        }

        public static IHtmlString RenderScriptManagerEntries(this HtmlHelper html)
        {
            var scriptManager = ScriptManager.GetFromViewData(html.ViewData);
            var entries = scriptManager.GetEntries();

            var sb = new StringBuilder();
            sb.AppendLine().AppendLine("<-- Begin ScriptManager Entries -->");
            foreach (var entry in entries)
            {
                var renderedEntry = FirtsNotNull(entry, RenderCssReference, RenderJsReference, RenderGlobalJavascript);
                if (renderedEntry != null)
                    sb.AppendLine(renderedEntry);
            }
            sb.AppendLine("<-- End ScriptManager Entries -->");
            return new HtmlString(sb.ToString());
        }

        private static string RenderCssReference(ScriptManagerEntry entry)
        {
            var casted = entry as CssReferenceEntry;
            if (casted == null)
                return null;
            return string.Format(@"<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />", casted.Path);
        }

        private static string RenderJsReference(ScriptManagerEntry entry)
        {
            var casted = entry as JsReferenceEntry;
            if (casted == null)
                return null;
            return string.Format(@"<script src=""{0}"" type=""text/javascript""></script>", casted.Path);
        }

        private static string RenderGlobalJavascript(ScriptManagerEntry entry)
        {
            var casted = entry as GlobalJavascriptEntry;
            if (casted == null)
                return null;
            return string.Format(@"<script type=""text/javascript"">window.{0} = {1};</script>", casted.Name, new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(casted.Value));
        }

    }
}
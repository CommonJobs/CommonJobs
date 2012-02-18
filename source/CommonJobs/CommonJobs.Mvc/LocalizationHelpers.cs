using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc
{
    public static class LocalizationHelpers
    {
        public static IHtmlString MetaAcceptLanguage<T>(this HtmlHelper<T> html)
        {
            var acceptLanguage = HttpUtility.HtmlAttributeEncode(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());
            return new HtmlString(String.Format("<meta name=\"accept-language\" content=\"{0}\">", acceptLanguage));
        }
    }
}

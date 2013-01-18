using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace CommonJobs.Infrastructure.Mvc.SecurityTrimming
{
    public static class SecurityTrimmingExtensions
    {
        public static SecurityTrimmingHelper GetSecurityTrimmingHelper(this HtmlHelper htmlHelper, string actionName, string controllerName = null)
        {
            return new SecurityTrimmingHelper(htmlHelper, actionName, controllerName);
        }

        public static SecurityTrimmingForm BeginSecurityTrimmingForm(this HtmlHelper htmlHelper, string actionName, string controllerName = null)
        {
            Func<MvcForm> lazyForm = () => htmlHelper.BeginForm(actionName, controllerName);
            return new SecurityTrimmingForm(htmlHelper, lazyForm, actionName, controllerName);
        }

        public static HelperResult RenderIfHasPermission(this HtmlHelper htmlHelper, string actionName, Func<SecurityTrimmingHelper, HelperResult> template)
        {
            return RenderIfHasPermission(htmlHelper, actionName, null, null, template);
        }

        public static HelperResult RenderIfHasPermission(this HtmlHelper htmlHelper, string actionName, string controllerName, Func<SecurityTrimmingHelper, HelperResult> template)
        {
            return RenderIfHasPermission(htmlHelper, actionName, controllerName, null, template);
        }

        public static HelperResult RenderIfHasPermission(this HtmlHelper htmlHelper, string actionName, string controllerName, string areaName, Func<SecurityTrimmingHelper, HelperResult> template)
        {
            using (var helper = new SecurityTrimmingHelper(htmlHelper, actionName, controllerName, areaName))
            {
                return helper.HasPermission ? template(helper) : new HelperResult(writer => { });
            }
        }
    }
}

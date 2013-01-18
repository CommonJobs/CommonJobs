using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using CommonJobs.Infrastructure.Mvc.SecurityTrimming;
using System.Web.Mvc.Html;

namespace CommonJobs.Mvc.UI.HtmlHelpers
{
    public static class BootstrapHelpers
    {
        public static HelperResult NavItem(this HtmlHelper Html, string linkText, string actionName, string controllerName = null, string areaName = null)
        {
            return Html.RenderIfHasPermission(
                actionName,
                controllerName,
                areaName,
                item => Html.NavItem(item.IsActiveController, Html.ActionLink(
                    linkText,
                    item.ActionName,
                    item.ControllerName,
                    item.AreaName != null ? new { area = item.AreaName } : null,
                    null).ToHtmlString()));
        }

        public static HelperResult NavItemAction(this HtmlHelper Html, string linkText, string actionName, string controllerName = null)
        {
            return Html.RenderIfHasPermission(
                actionName,
                controllerName,
                item => Html.NavItem(item.IsActiveAction, Html.ActionLink(linkText, item.ActionName, item.ControllerName).ToHtmlString()));
        }

        //TODO: I do not like it
        private static HelperResult NavItem(this HtmlHelper Html, bool isActive, string content)
        {
            return new HelperResult(writer =>
            {
                writer.Write("<li");
                if (isActive)
                {
                    writer.Write(" class='active'");
                }
                writer.Write(">");
                writer.Write(content);
                writer.Write("</li>");
            });
        }

        //TODO: I do not like it
        private static HelperResult NavItem(this HtmlHelper Html, bool isActive, Func<dynamic, HelperResult> template)
        { 
            return Html.NavItem(isActive, template(null).ToHtmlString());
        }
        
    }
}
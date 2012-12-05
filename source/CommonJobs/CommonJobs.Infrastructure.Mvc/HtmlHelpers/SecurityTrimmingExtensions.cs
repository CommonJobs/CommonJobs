using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages;

namespace CommonJobs.Infrastructure.Mvc.HtmlHelpers
{
    public class IfPermissionsItem
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsActiveController { get; set; }
        public bool IsActiveAction { get; set; }
    }

    public static class SecurityTrimmingExtensions
    {
        public static HelperResult HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName, Func<IfPermissionsItem, HelperResult> templateTrue, Func<IfPermissionsItem, HelperResult> templateFalse = null)
        {
            //TODO: support null controller
            var template = htmlHelper.HasActionPermission(actionName, controllerName) ? templateTrue : templateFalse;
            return template != null
                ? new HelperResult(writer => template(new IfPermissionsItem() { 
                    ActionName = actionName, 
                    ControllerName = controllerName,
                    IsActiveAction = IsActiveAction(htmlHelper, actionName, controllerName),
                    IsActiveController = IsActiveController(htmlHelper, controllerName)
                }).WriteTo(writer))
                : new HelperResult(writer => { });
        }

        public static HelperResult HasActionPermission(this HtmlHelper htmlHelper, string actionName, Func<IfPermissionsItem, HelperResult> templateTrue, Func<IfPermissionsItem, HelperResult> templateFalse = null)
        {
            return HasActionPermission(htmlHelper, actionName, null, templateTrue, templateFalse);
        }


        public static bool IsActiveController(this HtmlHelper htmlHelper, string controllerName = null)
        {
            if (controllerName == null)
                return true;

            var routeData = htmlHelper.ViewContext.RouteData.Values;
            var currentController = routeData["controller"];
            return String.Equals(controllerName, currentController as string, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsActiveAction(this HtmlHelper htmlHelper, string actionName, string controllerName = null)
        {
            if (!IsActiveController(htmlHelper, controllerName))
                return false;

            var routeData = htmlHelper.ViewContext.RouteData.Values;
            var currentAction = routeData["action"];
            return String.Equals(actionName, currentAction as string, StringComparison.OrdinalIgnoreCase);
        }

        //FROM: http://stackoverflow.com/questions/2721869/security-aware-action-link
        public static bool HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName = null)
        {
            //if the controller name is empty the ASP.NET convention is:
            //"we are linking to a different controller
            ControllerBase controllerToLinkTo = string.IsNullOrEmpty(controllerName)
                                                    ? htmlHelper.ViewContext.Controller
                                                    : GetControllerByName(htmlHelper, controllerName);

            var controllerContext = new ControllerContext(htmlHelper.ViewContext.RequestContext, controllerToLinkTo);

            var controllerDescriptor = new ReflectedControllerDescriptor(controllerToLinkTo.GetType());

            var actionDescriptor = controllerDescriptor.GetCanonicalActions().Where(x => String.Equals(x.ActionName, actionName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            //Originally it was: //var actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);
            //I changed it because, I want to check accessibility to an POST action. Maybe it could fail for actions for different http method and the same name

            
            return ActionIsAuthorized(controllerContext, actionDescriptor);
        }


        private static bool ActionIsAuthorized(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
                return false; // action does not exist so say yes - should we authorise this?!

            AuthorizationContext authContext = new AuthorizationContext(controllerContext, actionDescriptor);

            var authFilters = FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor).Select(x => x.Instance).OfType<IAuthorizationFilter>();
            // run each auth filter until on fails
            // performance could be improved by some caching
            foreach (var authFilter in authFilters)
            {
                authFilter.OnAuthorization(authContext);

                if (authContext.Result != null)
                    return false;
            }

            return true;
        }

        private static ControllerBase GetControllerByName(HtmlHelper helper, string controllerName)
        {
            // Instantiate the controller and call Execute
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

            IController controller = factory.CreateController(helper.ViewContext.RequestContext, controllerName);

            if (controller == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        "Controller factory {0} controller {1} returned null",
                        factory.GetType(),
                        controllerName));
            }

            return (ControllerBase)controller;
        }

    }
}

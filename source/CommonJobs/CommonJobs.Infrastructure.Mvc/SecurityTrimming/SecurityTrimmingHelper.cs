using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CommonJobs.Infrastructure.Mvc.SecurityTrimming
{
    public class SecurityTrimmingHelper : IDisposable
    {
        public HtmlHelper HtmlHelper { get; private set; }
        public string ControllerName { get; private set; }
        public string ActionName { get; private set; }
        public string AreaName { get; private set; }
        public bool IsActiveController { get; private set; }
        public bool IsActiveAction { get; private set; }
        public bool HasPermission { get; private set; }

        internal SecurityTrimmingHelper(HtmlHelper htmlHelper, string actionName, string controllerName = null, string areaName = null)
        {
            //TODO: better supprt to controllerName == null
            HtmlHelper = htmlHelper;
            ControllerName = controllerName;
            ActionName = actionName;
            AreaName = areaName;

            IsActiveController = CheckIsActiveController(htmlHelper.ViewContext, controllerName, areaName);
            IsActiveAction = IsActiveController && CheckIsActiveAction(htmlHelper.ViewContext, actionName, controllerName, areaName);
            HasPermission = CheckHasActionPermission(htmlHelper.ViewContext, actionName, controllerName, areaName != null);
        }

        public static bool CheckIsActiveController(ViewContext viewContext, string controllerName = null, string areaName = null)
        {
            //TODO: check area
            if (controllerName == null)
                return true;
            var currentController = viewContext.RouteData.Values["controller"];
            return String.Equals(controllerName, currentController as string, StringComparison.OrdinalIgnoreCase);
        }

        public static bool CheckIsActiveAction(ViewContext viewContext, string actionName, string controllerName = null, string areaName = null)
        {
            //TODO: check area
            if (!CheckIsActiveController(viewContext, controllerName))
                return false;
            var currentAction = viewContext.RouteData.Values["action"];
            return String.Equals(actionName, currentAction as string, StringComparison.OrdinalIgnoreCase);
        }

        //FROM: http://stackoverflow.com/questions/2721869/security-aware-action-link
        public static bool CheckHasActionPermission(ViewContext viewContext, string actionName, string controllerName = null, bool useNamespaceFallback = false)
        {
            //if the controller name is empty the ASP.NET convention is:
            //"we are linking to a different controller
            ControllerBase controllerToLinkTo = string.IsNullOrEmpty(controllerName)
                                                    ? viewContext.Controller
                                                    : GetControllerByName(viewContext, controllerName, useNamespaceFallback);

            var controllerContext = new ControllerContext(viewContext.RequestContext, controllerToLinkTo);

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

        private static ControllerBase GetControllerByName(ViewContext viewContext, string controllerName, bool useNamespaceFallback)
        {
            // Instantiate the controller and call Execute
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

            if (useNamespaceFallback)
                viewContext.RequestContext.RouteData.DataTokens["UseNamespaceFallback"] = true;
            
            IController controller = factory.CreateController(viewContext.RequestContext, controllerName);

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

        public virtual void Dispose()
        {
            //NOTE: only to allow use of using in Razor
        }
    }
}

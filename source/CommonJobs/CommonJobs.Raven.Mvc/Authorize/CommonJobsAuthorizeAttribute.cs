using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Configuration;
using CommonJobs.Raven.Mvc.Authorize;

namespace CommonJobs.Raven.Mvc
{
    public class CommonJobsAuthorizeAttribute : AuthorizeAttribute
    {
        public static IAuthorizationBehavior AuthorizationBehavior { get; set; }
                
        private string ApplyMapping(string from, Func<IEnumerable<string>, IEnumerable<string>> mapping)
        {
            if (from == null)
                return null;

            var fromList = from.ToRoleList();
            var resultList = mapping(fromList);
            return resultList.ToRolesString();
        }

        public new string Roles
        {
            get
            {
                return AuthorizationBehavior == null ? base.Roles
                    : ApplyMapping(base.Roles, AuthorizationBehavior.AdGroupsToAppRoles);
            }
            set
            {
                base.Roles = AuthorizationBehavior == null ? value
                    : ApplyMapping(value, AuthorizationBehavior.AppRolesToAdGroups);
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var actionAlternatives = filterContext.ActionDescriptor.GetCustomAttributes(true).OfType<IAlternativeAuthorizationAttribute>();
            var controllerAlternatives = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(true).OfType<IAlternativeAuthorizationAttribute>();

            if (!actionAlternatives.Union(controllerAlternatives).Any(x => x.Authorize(filterContext)))
                base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            bool? result = AuthorizationBehavior == null ? null 
                : AuthorizationBehavior.OverrideAuthorize(this, httpContext);

            return result.HasValue ? result.Value
                : base.AuthorizeCore(httpContext);
        }
    }
}

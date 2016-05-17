using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    public class SessionAndExternalRolesAuthorizationBehavior : IAuthorizationBehavior
    {
        private readonly Func<string, string[]> GetRoles;

        public SessionAndExternalRolesAuthorizationBehavior(Func<string, string[]> getRoles)
        {
            GetRoles = getRoles;
        }


        public IEnumerable<string> AdGroupsToAppRoles(IEnumerable<string> groups)
        {
            return groups;
        }

        public IEnumerable<string> AppRolesToAdGroups(IEnumerable<string> roles)
        {
            return roles;
        }

        public bool? OverrideAuthorize(CommonJobsAuthorizeAttribute authorizeAttribute, System.Web.HttpContextBase httpContext)
        {
            if (httpContext.User == null || httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                return false;

            if (String.IsNullOrEmpty(authorizeAttribute.Roles))
                return true;

            if (httpContext.Items["UserRoles"] == null)
            {
                httpContext.Items["UserRoles"] = GetRoles(httpContext.User.Identity.Name);
            }

            var sessionRoles = (string[])httpContext.Items["UserRoles"];

            var required = authorizeAttribute.Roles.ToRoleList();
            return sessionRoles.Intersect(required).Any();
        }
    }
}

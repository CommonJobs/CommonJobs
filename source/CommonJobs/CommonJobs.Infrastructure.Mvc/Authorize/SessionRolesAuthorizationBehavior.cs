using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    public class SessionRolesAuthorizationBehavior : IAuthorizationBehavior
    {
        public readonly string SessionRolesKey;

        public SessionRolesAuthorizationBehavior(string sessionRolesKey)
        {
            SessionRolesKey = sessionRolesKey;
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

            var sessionRoles = (string[])httpContext.Session[SessionRolesKey] ?? new string[] { };
            var required = authorizeAttribute.Roles.ToRoleList();
            return sessionRoles.Intersect(required).Any();
        }
    }
}

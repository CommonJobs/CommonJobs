using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    public class SessionAndExternalRolesAuthorizationBehavior : IAuthorizationBehavior
    {
        public readonly string SessionRolesKey;
        private readonly Func<string, string[]> GetRoles;

        public SessionAndExternalRolesAuthorizationBehavior(string sessionRolesKey, Func<string, string[]> getRoles)
        {
            SessionRolesKey = sessionRolesKey;
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

            var sessionRoles = httpContext.Session[SessionRolesKey] as string[];

            if (sessionRoles == null)
            {
                sessionRoles = GetRoles(httpContext.User.Identity.Name);
                httpContext.Session[SessionRolesKey] = sessionRoles;
            }
                            
            var required = authorizeAttribute.Roles.ToRoleList();
            return sessionRoles.Intersect(required).Any();
        }
    }
}

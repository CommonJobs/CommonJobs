using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    public class MixedAuthorizationBehavior : IAuthorizationBehavior
    {
        readonly IAuthorizationBehavior SessionRolesAuthorizationBehavior;
        readonly IAuthorizationBehavior PrefixAuthorizationBehavior;

        public MixedAuthorizationBehavior(SessionRolesAuthorizationBehavior sessionRolesAuthorizationBehavior, PrefixAuthorizationBehaviorBase prefixAuthorizationBehavior)
        {
            SessionRolesAuthorizationBehavior = sessionRolesAuthorizationBehavior;
            PrefixAuthorizationBehavior = prefixAuthorizationBehavior;
        }

        public IEnumerable<string> AdGroupsToAppRoles(IEnumerable<string> groups)
        {
            return PrefixAuthorizationBehavior.AdGroupsToAppRoles(groups);
        }

        public IEnumerable<string> AppRolesToAdGroups(IEnumerable<string> roles)
        {
            return PrefixAuthorizationBehavior.AppRolesToAdGroups(roles);
        }

        public bool? OverrideAuthorize(CommonJobsAuthorizeAttribute authorizeAttribute, System.Web.HttpContextBase httpContext)
        {
            if (httpContext.User != null && httpContext.User.Identity != null && httpContext.User.Identity.AuthenticationType == "Forms")
                return SessionRolesAuthorizationBehavior.OverrideAuthorize(authorizeAttribute, httpContext);
            else
                return PrefixAuthorizationBehavior.OverrideAuthorize(authorizeAttribute, httpContext);
        }
    }
}

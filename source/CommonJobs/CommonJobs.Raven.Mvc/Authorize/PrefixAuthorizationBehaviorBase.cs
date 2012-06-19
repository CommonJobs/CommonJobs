using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Raven.Mvc.Authorize
{
    public abstract class PrefixAuthorizationBehaviorBase : IAuthorizationBehavior
    {
        public abstract string Prefix { get; }

        IEnumerable<string> IAuthorizationBehavior.AdGroupsToAppRoles(IEnumerable<string> groups)
        {
            return groups.Select(x => x.StartsWith(Prefix) ? x.Substring(Prefix.Length) : x);
        }

        IEnumerable<string> IAuthorizationBehavior.AppRolesToAdGroups(IEnumerable<string> roles)
        {
            return roles.Select(x => string.Format("{0}{1}", Prefix, x));
        }

        bool? IAuthorizationBehavior.OverrideAuthorize(CommonJobsAuthorizeAttribute authorizeAttribute, System.Web.HttpContextBase httpContext)
        {
            return null;
        }
    }
}

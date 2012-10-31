using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    public abstract class ForcedGroupsAuthorizationBehaviorBase : IAuthorizationBehavior
    {
        public abstract HashSet<string> ForcedGroups { get; }

        IEnumerable<string> IAuthorizationBehavior.AdGroupsToAppRoles(IEnumerable<string> groups)
        {
            return groups;
        }

        IEnumerable<string> IAuthorizationBehavior.AppRolesToAdGroups(IEnumerable<string> roles)
        {
            return roles;
        }

        bool? IAuthorizationBehavior.OverrideAuthorize(CommonJobsAuthorizeAttribute authorizeAttribute, System.Web.HttpContextBase httpContext)
        {
            //No setear la configuración representa un usuario no autenticado
            if (ForcedGroups == null)
                return false;

            if (String.IsNullOrEmpty(authorizeAttribute.Roles))
                return true;
            
            var required = authorizeAttribute.Roles.ToRoleList();

            return ForcedGroups.Intersect(required).Any();
        }
    }
}

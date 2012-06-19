using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CommonJobs.Raven.Mvc.Authorize
{
    public interface IAuthorizationBehavior
    {
        IEnumerable<string> AdGroupsToAppRoles(IEnumerable<string> groups);
        IEnumerable<string> AppRolesToAdGroups(IEnumerable<string> roles);
        bool? OverrideAuthorize(CommonJobsAuthorizeAttribute authorizeAttribute, HttpContextBase httpContext);
    }
}

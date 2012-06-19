using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Raven.Mvc.Authorize
{
    public class ForcedGroupsAuthorizationBehavior : ForcedGroupsAuthorizationBehaviorBase
    {
        private HashSet<string> forcedGroups;

        public override HashSet<string> ForcedGroups 
        {
            get { return forcedGroups; }
        }

        public ForcedGroupsAuthorizationBehavior(HashSet<string> forcedGroups)
        {
            this.forcedGroups = forcedGroups;
        }

        public ForcedGroupsAuthorizationBehavior(string forcedGroups)
        {
            this.forcedGroups = new HashSet<string>(forcedGroups.ToRoleList());
        }
    }
}

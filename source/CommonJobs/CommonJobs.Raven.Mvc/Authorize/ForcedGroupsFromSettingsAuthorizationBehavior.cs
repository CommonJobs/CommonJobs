using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonJobs.Raven.Mvc.Authorize
{
    public class ForcedGroupsFromSettingsAuthorizationBehavior : ForcedGroupsAuthorizationBehaviorBase
    {
        private string settingKey;

        public override HashSet<string> ForcedGroups 
        {
            get 
            { 
                var setting = ConfigurationManager.AppSettings[settingKey];

                if (setting == null)
                    return null;

                return new HashSet<string>(setting.ToRoleList()); ; 
            }
        }

        public ForcedGroupsFromSettingsAuthorizationBehavior(string settingKey)
        {
            this.settingKey = settingKey;
        }
    }
}

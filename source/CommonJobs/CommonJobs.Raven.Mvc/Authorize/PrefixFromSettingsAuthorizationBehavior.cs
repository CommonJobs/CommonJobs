using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CommonJobs.Raven.Mvc.Authorize
{
    public class PrefixFromSettingsAuthorizationBehavior : PrefixAuthorizationBehaviorBase
    {
        private string settingKey;

        public override string Prefix 
        {
            get { return ConfigurationManager.AppSettings[settingKey] ?? string.Empty; }
        }

        public PrefixFromSettingsAuthorizationBehavior(string settingKey)
        {
            this.settingKey = settingKey;
        }
    }
}

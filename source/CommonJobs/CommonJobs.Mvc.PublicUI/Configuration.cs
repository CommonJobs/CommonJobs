using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.PublicUI
{
    public static class Configuration
    {
        private static string GetConfigurationByKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string TemporalUploadsPath
        {
            get
            {
                return GetConfigurationByKey("CommonJobs/TemporalUploadsPath");
            }
        }
    }
}
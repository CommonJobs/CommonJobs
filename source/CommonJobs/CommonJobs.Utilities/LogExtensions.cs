using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NLog;

namespace CommonJobs.Utilities
{
    public static class LogExtensions
    {
        public static void Dump(this Logger logger, LogLevel level, object obj, string message = null)
        {
            logger.Log(level, () => Dump(obj, message));
        }

        private static string Dump(object obj, string message = null)
        {
            return string.Format("{0}\n{1}", 
                message,
                JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}

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
        public static void Dump(this Logger logger, LogLevel level, object obj)
        {
            logger.Log(level, () => Dump(obj));
        }

        private static string Dump(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }
    }
}

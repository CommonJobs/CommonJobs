using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Raven.Mvc.Authorize
{
    internal static class InternalUtilities
    {
        public static string ToRolesString(this IEnumerable<string> resultList)
        {
            return string.Join(", ", resultList);
        }

        public static IEnumerable<string> ToRoleList(this string from)
        {
            return from.Split(new[] { ',' }).Select(x => x.Trim());
        }
    }
}

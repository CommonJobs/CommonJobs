using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Utilities
{
    public static class StringUtilities
    {
        public static string AppendIfDoesNotEndWith(this string originalString, string ending)
        {
            if (string.IsNullOrEmpty(originalString)) 
                return ending;

            if (originalString.EndsWith(ending))
                return originalString;

            return originalString + ending;
        }
    }
}

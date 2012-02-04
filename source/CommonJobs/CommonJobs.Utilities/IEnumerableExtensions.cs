using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Utilities
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns empty enumeration when source is null
        /// </summary>
        /// <rremarks>
        /// Perhaps it is not very elegant, but it will help us to keep our code clearer.
        /// </rremarks>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}

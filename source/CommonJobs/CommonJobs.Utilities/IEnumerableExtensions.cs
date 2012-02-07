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

        public static IEnumerable<IGrouping<int, T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            return source
                .Select((x, index) => new { Element = x, index = index })
                .GroupBy(x => x.index / batchSize, e => e.Element);
        }
    }
}

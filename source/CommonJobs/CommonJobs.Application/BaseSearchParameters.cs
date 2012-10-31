using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application
{
    public class BaseSearchParameters
    {
        public string Term { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        internal IQueryable<T> ApplyPagination<T>(IQueryable<T> query)
        {
            if (Skip > 0)
                query = query.Skip(Skip);
            if (Take > 0)
                query = query.Take(Take);
            return query;
        }
    }

    internal static class BaseSearchParametersExtensions
    {
        internal static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, BaseSearchParameters searchParameters)
        {
            return searchParameters.ApplyPagination(query);
        }
    }
}

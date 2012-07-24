using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Raven.Infrastructure;
using Raven.Client.Linq;

namespace CommonJobs.Infrastructure.JobSearchSearching
{
    public class SearchJobSearches: Query<JobSearchSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        JobSearchSearchParameters Parameters { get; set; }

        public SearchJobSearches(JobSearchSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override JobSearchSearchResult[] Execute()
        {
            RavenQueryStatistics stats;
            IQueryable<JobSearch_QuickSearch.Projection> query = RavenSession
                .Query<JobSearch_QuickSearch.Projection, JobSearch_QuickSearch>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            query = query.Where(x => x.IsJobSearch);

            //TODO set up criteria for real parameters
            Expression<Func<JobSearch_QuickSearch.Projection, bool>> predicate = x =>
                x.Title.StartsWith(Parameters.Term);

            //TODO set order -- whichever order works ok for now

            if (Parameters.Skip > 0)
                query = query.Skip(Parameters.Skip);

            if (Parameters.Take > 0)
                query = query.Take(Parameters.Take);

            var result = query.AsProjection<JobSearchSearchResult>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

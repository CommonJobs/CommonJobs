using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Raven.Infrastructure;
using Raven.Client.Linq;

namespace CommonJobs.Infrastructure.JobSearchSearching
{
    public class SearchJobSearches: Query<JobSearch[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        JobSearchSearchParameters Parameters { get; set; }

        public SearchJobSearches(JobSearchSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override JobSearch[] Execute()
        {
            RavenQueryStatistics stats;

            IQueryable<JobSearch> query = RavenSession
                .Query<JobSearch>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            //TODO set up criteria for real parameters

            //TODO set order -- whichever order works ok for now
            if (Parameters.Skip > 0)
                query = query.Skip(Parameters.Skip);

            if (Parameters.Take > 0)
                query = query.Take(Parameters.Take);

            var result = query.ToArray();
            Stats = stats;
            return result;
        }
    }
}

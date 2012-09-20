using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Linq;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Domain;
using CommonJobs.ContentExtraction;
using CommonJobs.Infrastructure.Indexes;
using System.Linq.Expressions;
using CommonJobs.Utilities;

namespace CommonJobs.Infrastructure.Vacations
{
    public class SearchVacations : Query<VacationsSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        BaseSearchParameters Parameters { get; set; }

        public SearchVacations(BaseSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override VacationsSearchResult[] Execute()
        {
            RavenQueryStatistics stats;

            var query = RavenSession
                .Query<Employee>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .AsProjection<VacationsSearchResult>()
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ApplyPagination(Parameters);
            
            var result = query.ToArray();
            Stats = stats;
            return result;
        }
    }
}

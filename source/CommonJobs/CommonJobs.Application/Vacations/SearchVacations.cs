using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Linq;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Domain;
using CommonJobs.ContentExtraction;
using CommonJobs.Application.Indexes;
using System.Linq.Expressions;
using CommonJobs.Utilities;

namespace CommonJobs.Application.Vacations
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

            var query1 = RavenSession
                .Query<Employee>();

            if (!string.IsNullOrWhiteSpace(Parameters.Term))
            {
                query1 = query1.Where(x => x.LastName.StartsWith(Parameters.Term) || x.FirstName.StartsWith(Parameters.Term));
            }

            var query2 = query1
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .AsProjection<VacationsSearchResult>()
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ApplyPagination(Parameters);
            
            var result = query2.ToArray();
            Stats = stats;
            return result;
        }
    }
}

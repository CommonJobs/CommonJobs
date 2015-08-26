using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Linq;
using CommonJobs.Infrastructure.RavenDb;
using CommonJobs.Domain;
using CommonJobs.ContentExtraction;
using CommonJobs.Application.Indexes;
using System.Linq.Expressions;
using CommonJobs.Utilities;
using Raven.Client;

namespace CommonJobs.Application.ApplicantSearching
{
    public class SearchApplicants : Query<ApplicantSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        ApplicantSearchParameters Parameters { get; set; }

        public SearchApplicants(ApplicantSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override ApplicantSearchResult[] Execute()
        {
            RavenQueryStatistics stats;
            IQueryable<Applicant_QuickSearch.Projection> query = RavenSession
                .Query<Applicant_QuickSearch.Projection, Applicant_QuickSearch>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            query = query
                .OrderByDescending(x => x.IsHighlighted)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName);

            query = Parameters.Apply(query);

            //var result = query.AsProjection<ApplicantSearchResult>().ToArray();
            var result = query.As<ApplicantSearchResult>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

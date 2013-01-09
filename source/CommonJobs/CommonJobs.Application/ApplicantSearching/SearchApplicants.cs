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

            query = query.Where(x => x.IsApplicant);

            Expression<Func<Applicant_QuickSearch.Projection, bool>> predicate = x =>
                x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Companies.Any(y => y.StartsWith(Parameters.Term))
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.TechnicalSkills.Any(y => y.StartsWith(Parameters.Term))
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term));

            if (Parameters.SearchInAttachments)
                predicate = predicate.Or(x => x.AttachmentContent.Any(y => y.StartsWith(Parameters.Term)));

            query = query.Where(predicate);

            foreach (var slug in Parameters.WithEvents.EmptyIfNull())
                query = query.Where(x => x.EventSlugs == slug);

            if (Parameters.Highlighted)
                query = query.Where(x => x.IsHighlighted);

            query = query
                .OrderByDescending(x => x.IsHighlighted)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName);

            if (Parameters.Skip > 0)
                query = query.Skip(Parameters.Skip);

            if (Parameters.Take > 0)
                query = query.Take(Parameters.Take);

            var result = query.AsProjection<ApplicantSearchResult>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

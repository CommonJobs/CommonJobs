using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using CommonJobs.Utilities;

namespace CommonJobs.Application.ApplicantSearching
{
    public class ApplicantSearchParameters : BaseSearchParameters
    {
        public bool Highlighted { get; set; }
        public bool SearchInAttachments { get; set; }
        public string[] WithEvents { get; set; }
        public ApplicantHiredFilter Hired { get; set; }

        public IQueryable<Applicant_QuickSearch.Projection> Apply(IQueryable<Applicant_QuickSearch.Projection> query)
        {
            return this.ApplyPagination(Filter(query));
        }

        private IQueryable<Applicant_QuickSearch.Projection> Filter(IQueryable<Applicant_QuickSearch.Projection> query)
        {
            query = query.Where(x => x.IsApplicant);

            if (Hired == ApplicantHiredFilter.Exclude)
            {
                query = query.Where(x => !x.IsHired);
            }
            else if (Hired == ApplicantHiredFilter.OnlyHired)
            {
                query = query.Where(x => x.IsHired);
            }

            Expression<Func<Applicant_QuickSearch.Projection, bool>> predicate = x =>
                            x.FullName1.StartsWith(Term)
                                || x.FullName2.StartsWith(Term)
                                || x.Companies.Any(y => y.StartsWith(Term))
                                || x.Skills.StartsWith(Term)
                                || x.TechnicalSkills.Any(y => y.StartsWith(Term))
                                || x.AttachmentNames.Any(y => y.StartsWith(Term));

            if (SearchInAttachments)
                predicate = predicate.Or(x => x.AttachmentContent.Any(y => y.StartsWith(Term)));

            query = query.Where(predicate);

            foreach (var slug in WithEvents.EmptyIfNull())
                query = query.Where(x => x.EventSlugs == slug);

            if (Highlighted)
                query = query.Where(x => x.IsHighlighted);

            return query;
        }
    }

    public enum ApplicantHiredFilter
    {
        Exclude = 0,
        Include = 1,
        OnlyHired = 2
    }
}
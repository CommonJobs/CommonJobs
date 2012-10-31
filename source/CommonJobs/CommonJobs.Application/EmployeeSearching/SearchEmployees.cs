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

namespace CommonJobs.Application.EmployeeSearching
{
    public class SearchEmployees : Query<EmployeeSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        EmployeeSearchParameters Parameters { get; set; }

        public SearchEmployees(EmployeeSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override EmployeeSearchResult[] Execute()
        {
            RavenQueryStatistics stats;
            IQueryable<Employee_QuickSearch.Projection> query = RavenSession
                .Query<Employee_QuickSearch.Projection, Employee_QuickSearch>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            query = query.Where(x => x.IsEmployee);

            Expression<Func<Employee_QuickSearch.Projection, bool>> predicate = x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term))
                    || x.CurrentPosition.StartsWith(Parameters.Term)
                    || x.CurrentProject.StartsWith(Parameters.Term)
                    || x.FileId.StartsWith(Parameters.Term)
                    || x.Platform.StartsWith(Parameters.Term)
                    || x.Terms.StartsWith(Parameters.Term);

            if (Parameters.SearchInNotes)
                predicate = predicate.Or(x => x.Notes.StartsWith(Parameters.Term));

            if (Parameters.SearchInAttachments)
                predicate = predicate.Or(x => x.AttachmentContent.Any(y => y.StartsWith(Parameters.Term)));

            query = query.Where(predicate).OrderBy(x => x.LastName).ThenBy(x => x.FirstName);

            if (Parameters.Skip > 0)
                query = query.Skip(Parameters.Skip);

            if (Parameters.Take > 0)
                query = query.Take(Parameters.Take);

            var result = query.AsProjection<EmployeeSearchResult>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

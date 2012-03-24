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

namespace CommonJobs.Infrastructure.EmployeeSearching
{
    public class SearchEmployees : Query<EmployeeSearchResult[]>
    {
        //TODO: add pagination support
        EmployeeSearchParameters Parameters { get; set; }

        public SearchEmployees(EmployeeSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override EmployeeSearchResult[] Execute()
        {
            var query = RavenSession
                .Query<Employee_QuickSearch.Projection, Employee_QuickSearch>()
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
            
            query = query.Where(predicate).OrderBy(x => x.FullName1);

            var result = query.AsProjection<EmployeeSearchResult>().ToArray();
            return result;
        }
    }
}

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

            //TODO: enhance it
            if (!Parameters.SearchInAttachments && !Parameters.SearchInNotes)
                query = query.Where(x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term))
                    || x.CurrentPosition.StartsWith(Parameters.Term)
                    || x.CurrentProject.StartsWith(Parameters.Term)
                    || x.FileId.StartsWith(Parameters.Term)
                    || x.Platform.StartsWith(Parameters.Term)
                    || x.Terms.StartsWith(Parameters.Term));
            else if (!Parameters.SearchInAttachments && Parameters.SearchInNotes)
                query = query.Where(x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term))
                    || x.CurrentPosition.StartsWith(Parameters.Term)
                    || x.CurrentProject.StartsWith(Parameters.Term)
                    || x.FileId.StartsWith(Parameters.Term)
                    || x.Platform.StartsWith(Parameters.Term)
                    || x.Terms.StartsWith(Parameters.Term)
                    || x.Notes.StartsWith(Parameters.Term));
            else if (Parameters.SearchInAttachments && !Parameters.SearchInNotes)
                query = query.Where(x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term))
                    || x.CurrentPosition.StartsWith(Parameters.Term)
                    || x.CurrentProject.StartsWith(Parameters.Term)
                    || x.FileId.StartsWith(Parameters.Term)
                    || x.Platform.StartsWith(Parameters.Term)
                    || x.Terms.StartsWith(Parameters.Term)
                    || x.AttachmentContent.Any(y => y.StartsWith(Parameters.Term)));
            else if (!Parameters.SearchInAttachments && Parameters.SearchInNotes)
                query = query.Where(x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term))
                    || x.CurrentPosition.StartsWith(Parameters.Term)
                    || x.CurrentProject.StartsWith(Parameters.Term)
                    || x.FileId.StartsWith(Parameters.Term)
                    || x.Platform.StartsWith(Parameters.Term)
                    || x.Terms.StartsWith(Parameters.Term)
                    || x.Notes.StartsWith(Parameters.Term)
                    || x.AttachmentContent.Any(y => y.StartsWith(Parameters.Term)));

            query = query.OrderBy(x => x.FullName1);

            var result = query.AsProjection<EmployeeSearchResult>().ToArray();
            return result;
        }
    }
}

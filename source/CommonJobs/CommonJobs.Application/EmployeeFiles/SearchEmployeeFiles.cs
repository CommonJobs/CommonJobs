using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using Raven.Client;

namespace CommonJobs.Application.EmployeeFiles
{
    public class SearchEmployeeFiles: Query<EmployeeFileSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        BaseSearchParameters Parameters { get; set; }

        public SearchEmployeeFiles(BaseSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override EmployeeFileSearchResult[] Execute()
        {
            RavenQueryStatistics stats;

            var query = RavenSession
                .Query<Employee>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.TerminationDate == null)
                //.AsProjection<EmployeeFileSearchResult>()
                .ProjectFromIndexFieldsInto<EmployeeFileSearchResult>()
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ApplyPagination(Parameters);

            var result = query.ToArray();
            Stats = stats;
            return result;
        }
    }
}

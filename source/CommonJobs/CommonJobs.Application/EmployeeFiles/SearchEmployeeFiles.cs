using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using Raven.Client;
using CommonJobs.Application.EmployeeSearching;

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
                .Query<Employee_QuickSearch.Projection, Employee_QuickSearch>()
                .Where(x => x.IsEmployee);

            var rs = query
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ApplyPagination(Parameters)
                .As<EmployeeFileSearchResult>()
                .ToArray();

            Stats = stats;
            return rs;
        }
    }
}

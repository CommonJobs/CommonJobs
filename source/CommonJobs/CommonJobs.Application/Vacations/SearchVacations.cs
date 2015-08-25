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
using CommonJobs.Application.EmployeeSearching;
using Raven.Client;

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
                .Query<Employee_QuickSearch.Projection, Employee_QuickSearch>()
                .Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(Parameters.Term))
            {
                query1 = query1.Where(x => x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term));
            }

            var ids = query1
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ApplyPagination(Parameters)
                .Select(x => x.Id)
                .ToArray();

            var employees = RavenSession.Load<Employee>(ids);
            var results = employees.Select(x => new VacationsSearchResult()
            {
                FirstName = x.FirstName,
                HiringDate = x.HiringDate,
                Id = x.Id,
                LastName = x.LastName,
                Vacations = x.Vacations
            }).ToArray();

            Stats = stats;

            return results;
        }
    }
}

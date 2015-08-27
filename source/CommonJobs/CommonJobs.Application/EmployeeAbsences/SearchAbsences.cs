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

namespace CommonJobs.Application.EmployeeAbsences
{
    public class SearchAbsences : Query<AbsencesSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        BaseSearchParameters Parameters { get; set; }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public SearchAbsences(BaseSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override AbsencesSearchResult[] Execute()
        {
            RavenQueryStatistics stats;

            var query1 = RavenSession
                .Query<Employee_QuickSearch.Projection, Employee_QuickSearch>()
                .Where(x => x.IsEmployee);

            if (To.HasValue)
                query1 = query1.Where(x => x.HiringDate <= To.Value);

            if (From.HasValue)
                query1 = query1.Where(x => x.TerminationDate >= From.Value);

            if (!string.IsNullOrWhiteSpace(Parameters.Term))
            {
                query1 = query1.Where(x => x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term));
            }

            var rs = query1
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .ApplyPagination(Parameters)
                .As<EmployeeSearchResult>()
                .ToArray();

            var ids = rs.Select(x => x.Id).ToArray();

            var employees = RavenSession.Load<Employee>(ids);


            Func<Absence, bool> filterAbsencesTo;
            Func<Vacation, bool> filterVacationsTo;
            if (To.HasValue)
            {
                filterAbsencesTo = item => item.RealDate <= To.Value;
                filterVacationsTo = item => item.From <= To.Value;
            }
            else
            {
                filterAbsencesTo = item => true;
                filterVacationsTo = item => true;
            }

            Func<Absence, bool> filterAbsencesFrom;
            Func<Vacation, bool> filterVacationsFrom;
            if (From.HasValue)
            {
                filterAbsencesFrom = item => (item.To ?? item.RealDate) >= From.Value;
                filterVacationsFrom = item => item.To >= From.Value;
            }
            else
            {
                filterAbsencesFrom = item => true;
                filterVacationsFrom = item => true;
            }

            var results = employees
                .Select(x => new AbsencesSearchResult(
                    x,
                    x.Absences.Where(filterAbsencesTo).Where(filterAbsencesFrom).ToList(),
                    x.Vacations.Where(filterVacationsTo).Where(filterVacationsFrom).ToList()))
                .ToArray();

            Stats = stats;

            return results;
        }
    }
}

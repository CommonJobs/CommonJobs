using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Application.Persons;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using Raven.Client;

namespace CommonJobs.Application.EmployeeAbsences
{
    public class GetSuggestions : Query<IEnumerable<string>>
    {
        public string Term { get; set; }
        public int MaxSuggestions { get; set; }

        public GetSuggestions(string term, int maxSuggestions = 8)
        {
            Term = term;
            MaxSuggestions = maxSuggestions;
        }

        public override IEnumerable<string> Execute()
        {
            RavenQueryStatistics stats;

            var query = RavenSession.Query<EmployeeAbsences_Suggestions.Projection, EmployeeAbsences_Suggestions>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Search(x => x.Text, Term.TrimEnd('*', '?') + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
                .Distinct()
                .Take(MaxSuggestions);

            var results = query
                .ToList()
                .Select(x => x.Text)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(MaxSuggestions)
                .ToList();

            if (results.Count < MaxSuggestions)
            {
                var suggestionTerms = query.Suggest().Suggestions.Where(x => x != Term).ToArray();

                if (suggestionTerms.Length > 0)
                {
                    var extraQuery = RavenSession.Query<EmployeeAbsences_Suggestions.Projection, EmployeeAbsences_Suggestions>()
                        .Search(x => x.Text, string.Join(" ", suggestionTerms))
                        .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
                        .Distinct()
                        .Take(MaxSuggestions - results.Count);

                    var extraResults = extraQuery
                        .ToList()
                        .Select(x => x.Text)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Where(x => !results.Contains(x))
                        .Take(MaxSuggestions - results.Count)
                        .ToList();

                    results.AddRange(extraResults);
                }
            }

            return results;
        }
    }
}

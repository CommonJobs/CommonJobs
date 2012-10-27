using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Application.Persons;
using CommonJobs.Raven.Infrastructure;
using Raven.Client.Linq;

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
            var query = RavenSession.Query<EmployeeAbsences_Suggestions.Projection, EmployeeAbsences_Suggestions>()
                .Search(x => x.Text, Term.TrimEnd('*', '?') + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
                .Select(x => x.Text)
                .As<string>()
                .Distinct()
                .Take(MaxSuggestions * 2); //Padding porque no puedo filtrar los vacíos antes

            var results = query.AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Take(MaxSuggestions).ToList();

            if (results.Count < MaxSuggestions)
            {
                var suggestionTerms = query.Suggest().Suggestions.Where(x => x != Term).ToArray();
                if (suggestionTerms.Length > 0)
                {
                    var extraQuery = RavenSession.Query<EmployeeAbsences_Suggestions.Projection, EmployeeAbsences_Suggestions>()
                        .Search(x => x.Text, string.Join(" ", suggestionTerms))
                        .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
                        .Select(x => x.Text)
                        .As<string>()
                        .Distinct()
                        .Take((MaxSuggestions - results.Count) * 2); //Padding porque no puedo filtrar los vacíos y duplicados antes

                    var extraResults = extraQuery.ToList();

                    extraResults = extraResults
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

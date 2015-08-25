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

namespace CommonJobs.Application.Suggestions
{
    public class GetSuggestions : Query<IEnumerable<string>>
    {
        public Expression<Func<Persons_Suggestions.Projection, object>> FieldSelector { get; set; }
        public string Term { get; set; }
        public int MaxSuggestions { get; set; }

        public GetSuggestions(Expression<Func<Persons_Suggestions.Projection, object>> fieldSelector, string term, int maxSuggestions = 8)
        {
            FieldSelector = fieldSelector;
            Term = term;
            MaxSuggestions = maxSuggestions;
        }

        public override IEnumerable<string> Execute()
        {
            var query = RavenSession.Query<Persons_Suggestions.Projection, Persons_Suggestions>()
                .Search(FieldSelector, Term.TrimEnd('*', '?') + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .Select(FieldSelector)
                .As<string>()
                .Distinct()
                .Take(MaxSuggestions * 2); //Padding porque no puedo filtrar los vacíos antes

            var results = query.AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x)
                .Take(MaxSuggestions).ToList();

            if (results.Count < MaxSuggestions)
            {
                var suggestionTerms = query.Suggest().Suggestions.Where(x => x != Term).ToArray();
                if (suggestionTerms.Length > 0)
                {
                    var extraQuery = RavenSession.Query<Persons_Suggestions.Projection, Persons_Suggestions>()
                        .Search(FieldSelector, string.Join(" ", suggestionTerms))
                        .Select(FieldSelector)
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

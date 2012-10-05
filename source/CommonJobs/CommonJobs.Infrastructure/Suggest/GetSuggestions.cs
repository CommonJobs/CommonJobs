using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.Persons;
using CommonJobs.Raven.Infrastructure;
using Raven.Client.Linq;

namespace CommonJobs.Infrastructure.Suggestions
{
    public class GetSuggestions : Query<IEnumerable<string>>
    {
        public Expression<Func<Persons_Suggestions.Projection, object>> FieldSelector { get; set; }
        public string Term { get; set; }
        public int MaxSuggestions { get; set; }

        public GetSuggestions(Expression<Func<Persons_Suggestions.Projection, object>> fieldSelector, string term, int maxSuggestions = 10)
        {
            FieldSelector = fieldSelector;
            Term = term;
            MaxSuggestions = maxSuggestions;
        }

        public override IEnumerable<string> Execute()
        {
            var query = RavenSession.Query<Persons_Suggestions.Projection, Persons_Suggestions>()
                .Search(FieldSelector, Term + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .Select(FieldSelector)
                .As<string>()
                .Distinct()
                .Take(MaxSuggestions);

            var results = query.ToList();

            if (results.Count < MaxSuggestions)
            {
                var suggestionQueryResult = query.Suggest();

                var extraQuery = RavenSession.Query<Persons_Suggestions.Projection, Persons_Suggestions>()
                    .Search(FieldSelector, string.Join(" ", suggestionQueryResult.Suggestions))
                    .Select(FieldSelector)
                    .As<string>()
                    .Distinct()
                    .Take(MaxSuggestions - results.Count);

                var extraResults = extraQuery.ToList();

                results.AddRange(extraResults);
            }

            return results;
        }
    }
}

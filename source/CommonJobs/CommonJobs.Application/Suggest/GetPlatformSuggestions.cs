using CommonJobs.Infrastructure.RavenDb;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.Suggest
{
    public class GetPlatformSuggestions : Query<IEnumerable<string>>
    {
        public string Term { get; set; }
        public int MaxSuggestions { get; set; }

        public GetPlatformSuggestions(string term, int maxSuggestions = 8)
        {
            Term = term;
            MaxSuggestions = maxSuggestions;
        }

        public override IEnumerable<string> Execute()
        {
            var query = RavenSession.Query<Platform_Suggestions.Projection, Platform_Suggestions>()
                .Search(x => x.Platform, Term.TrimEnd('*', '?') + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .Distinct()
                .Take(MaxSuggestions * 2);

            var results = query
                .AsEnumerable()
                .Where(x => !string.IsNullOrWhiteSpace(x.Platform))
                .Take(MaxSuggestions)
                .ToList();

            if (results.Count < MaxSuggestions)
            {
                var suggestionTerms = query.Suggest().Suggestions.Where(x => x != Term).ToArray();
                if (suggestionTerms.Length > 0)
                {
                    var extraQuery = RavenSession.Query<Platform_Suggestions.Projection, Platform_Suggestions>()
                        .Search(x => x.Platform, string.Join(" ", suggestionTerms))
                        .Distinct()
                        .Take((MaxSuggestions - results.Count) * 2); //Padding porque no puedo filtrar los vacíos y duplicados antes

                    var extraResults = extraQuery.ToList();

                    extraResults = extraResults
                        .Where(x => !string.IsNullOrWhiteSpace(x.Platform))
                        .Where(x => !results.Contains(x))
                        .Take(MaxSuggestions - results.Count)
                        .ToList();

                    results.AddRange(extraResults);
                }
            }

            return results.Select(x => x.Platform).OrderBy(x => x).ToList();
        }
    }
}

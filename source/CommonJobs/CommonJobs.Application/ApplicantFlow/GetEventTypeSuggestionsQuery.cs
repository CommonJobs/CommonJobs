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

namespace CommonJobs.Application.ApplicantFlow
{
    public class GetEventTypeSuggestionsQuery : Query<IEnumerable<string>>
    {
        public string Term { get; set; }
        public int MaxSuggestions { get; set; }

        public GetEventTypeSuggestionsQuery(string term, int maxSuggestions = 8)
        {
            Term = term;
            MaxSuggestions = maxSuggestions;
        }

        public override IEnumerable<string> Execute()
        {
            RavenQueryStatistics stats;

            var q = RavenSession.Query<ApplicantEventType_Suggestions.Projection, ApplicantEventType_Suggestions>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Search(x => x.Text, Term.TrimEnd('*', '?') + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
                //.Select(FieldSelector)
                //.As<string>()
                .Distinct()
                .Take(MaxSuggestions * 2); //Padding porque no puedo filtrar los vacíos antes

            var query = q.ToList();

            var results = query.Select(x => x.Text).ToList();

            //var query = RavenSession.Query<ApplicantEventType_Suggestions.Projection, ApplicantEventType_Suggestions>()
            //    .Search(x => x.Text, Term.TrimEnd('*', '?') + "*", escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
            //    .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
            //    .Select(x => x.Text)
            //    .As<string>()
            //    .Distinct()
            //    .Take(MaxSuggestions * 2); //Padding porque no puedo filtrar los vacíos antes

            //var results = query.AsEnumerable()
            //    .Where(x => !string.IsNullOrWhiteSpace(x))
            //    .Take(MaxSuggestions).ToList();

            //if (results.Count < MaxSuggestions)
            //{
            //    var suggestionTerms = query.Suggest().Suggestions.Where(x => x != Term).ToArray();
            //    if (suggestionTerms.Length > 0)
            //    {
            //        var extraQuery = RavenSession.Query<ApplicantEventType_Suggestions.Projection, ApplicantEventType_Suggestions>()
            //            .Search(x => x.Text, string.Join(" ", suggestionTerms))
            //            .OrderByDescending(x => x.Predefined).ThenBy(x => x.Text)
            //            .Select(x => x.Text)
            //            .As<string>()
            //            .Distinct()
            //            .Take((MaxSuggestions - results.Count) * 2); //Padding porque no puedo filtrar los vacíos y duplicados antes

            //        var extraResults = extraQuery.ToList();

            //        extraResults = extraResults
            //            .Where(x => !string.IsNullOrWhiteSpace(x))
            //            .Where(x => !results.Contains(x))
            //            .Take(MaxSuggestions - results.Count)
            //            .ToList();

            //        results.AddRange(extraResults);
            //    }
            //}

            return results;
        }
    }
}

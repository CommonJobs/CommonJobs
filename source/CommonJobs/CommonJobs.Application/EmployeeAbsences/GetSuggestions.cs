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

            var q = RavenSession.Query<EmployeeAbsences_Suggestions.Projection, EmployeeAbsences_Suggestions>()
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

            return results;
        }
    }
}

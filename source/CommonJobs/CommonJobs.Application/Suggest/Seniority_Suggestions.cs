using CommonJobs.Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.Suggest
{
    public class Seniority_Suggestions : AbstractIndexCreationTask<Employee, Seniority_Suggestions.Projection>
    {
        public class Projection
        {
            public string Seniority { get; set; }
        }

        public Seniority_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    Seniority = entity.Seniority ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.Seniority into g
                             select new
                             {
                                 Seniority = g.Select(x => x.Seniority.Trim()).FirstOrDefault()
                             };

            Index(x => x.Seniority, FieldIndexing.Analyzed);
            Suggestion(x => x.Seniority);
        }
    }
}

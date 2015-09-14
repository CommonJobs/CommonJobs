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
    public class Degree_Suggestions : AbstractMultiMapIndexCreationTask<Degree_Suggestions.Projection>
    {
        public class Projection
        {
            public string Degree { get; set; }
        }

        public Degree_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    Degree = entity.Degree ?? string.Empty,
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    Degree = entity.Degree ?? string.Empty,
                });

            Reduce = docs => from doc in docs
                             group doc by doc.Degree into g
                             select new
                             {
                                 Degree = g.Select(x => x.Degree.Trim()).FirstOrDefault()
                             };

            Index(x => x.Degree, FieldIndexing.Analyzed);
            Suggestion(x => x.Degree);
        }
    }
}

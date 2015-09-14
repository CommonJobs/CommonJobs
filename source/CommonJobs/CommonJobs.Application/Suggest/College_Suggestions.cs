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
    public class College_Suggestions : AbstractMultiMapIndexCreationTask<College_Suggestions.Projection>
    {
        public class Projection
        {
            public string College { get; set; }
        }

        public College_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = entity.College ?? string.Empty,
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    College = entity.College ?? string.Empty,
                });

            Reduce = docs => from doc in docs
                group doc by doc.College into g
                select new
                {
                    College = g.Select(x => x.College.Trim()).FirstOrDefault()
                };

            Index(x => x.College, FieldIndexing.Analyzed);
            Suggestion(x => x.College);
        }
    }
}

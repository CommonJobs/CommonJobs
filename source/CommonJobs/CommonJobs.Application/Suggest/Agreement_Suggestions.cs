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
    public class Agreement_Suggestions : AbstractIndexCreationTask<Employee, Agreement_Suggestions.Projection>
    {
        public class Projection
        {
            public string Agreement { get; set; }
        }

        public Agreement_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    Agreement = entity.Agreement ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.Agreement into g
                             select new
                             {
                                 Agreement = g.Select(x => x.Agreement.Trim()).FirstOrDefault()
                             };

            Index(x => x.Agreement, FieldIndexing.Analyzed);
            Suggestion(x => x.Agreement);
        }
    }
}

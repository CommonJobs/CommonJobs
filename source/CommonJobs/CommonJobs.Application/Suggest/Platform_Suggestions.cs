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
    public class Platform_Suggestions : AbstractIndexCreationTask<Employee, Platform_Suggestions.Projection>
    {
        public class Projection
        {
            public string Platform { get; set; }
        }

        public Platform_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    Platform = entity.Platform ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.Platform into g
                             select new
                             {
                                 Platform = g.Select(x => x.Platform.Trim()).FirstOrDefault()
                             };

            Index(x => x.Platform, FieldIndexing.Analyzed);
            Suggestion(x => x.Platform);
        }
    }
}

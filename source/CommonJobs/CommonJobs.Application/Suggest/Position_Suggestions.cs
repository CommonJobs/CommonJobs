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
    public class Position_Suggestions : AbstractMultiMapIndexCreationTask<Position_Suggestions.Projection>
    {
        public class Projection
        {
            public string Position { get; set; }
        }

        public Position_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    Position = entity.InitialPosition ?? string.Empty,
                });

            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    Position = entity.CurrentPosition ?? string.Empty,
                });

            Reduce = docs => from doc in docs
                             group doc by doc.Position into g
                             select new
                             {
                                 Position = g.Select(x => x.Position.Trim()).FirstOrDefault()
                             };

            Index(x => x.Position, FieldIndexing.Analyzed);
            Suggestion(x => x.Position);
        }
    }
}

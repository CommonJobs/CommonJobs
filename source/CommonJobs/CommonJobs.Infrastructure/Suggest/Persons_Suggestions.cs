using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.Persons
{
    public class Persons_Suggestions : AbstractMultiMapIndexCreationTask<Persons_Suggestions.Projection>
    {
        public class Projection
        {
            public string College { get; set; }
            public string EntityType { get; set; }
            public string Id { get; set; }
        }

        public Persons_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = entity.College,
                    EntityType = "Employee",
                    Id = entity.Id
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    College = entity.College,
                    EntityType = "Applicant",
                    Id = entity.Id
                });

            Reduce = docs => from doc in docs
                             group doc by doc.Id into g
                             select new
                             {
                                College = g.Select(x => x.College).FirstOrDefault(),
                                EntityType = g.Select(x => x.EntityType).FirstOrDefault(),
                                Id = g.Select(x => x.Id).FirstOrDefault()
                             };
            
            Index(x => x.College, FieldIndexing.Analyzed);
        }
    }
}

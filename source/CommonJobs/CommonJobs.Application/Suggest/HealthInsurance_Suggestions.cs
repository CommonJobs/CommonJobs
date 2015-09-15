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
    public class HealthInsurance_Suggestions : AbstractIndexCreationTask<Employee, HealthInsurance_Suggestions.Projection>
    {
        public class Projection
        {
            public string HealthInsurance { get; set; }
        }

        public HealthInsurance_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    HealthInsurance = entity.HealthInsurance ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.HealthInsurance into g
                             select new
                             {
                                 HealthInsurance = g.Select(x => x.HealthInsurance.Trim()).FirstOrDefault()
                             };

            Index(x => x.HealthInsurance, FieldIndexing.Analyzed);
            Suggestion(x => x.HealthInsurance);
        }
    }
}

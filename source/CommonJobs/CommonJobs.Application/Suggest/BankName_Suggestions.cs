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
    public class BankName_Suggestions : AbstractIndexCreationTask<Employee, BankName_Suggestions.Projection>
    {
        public class Projection
        {
            public string BankName { get; set; }
        }

        public BankName_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    BankName = entity.BankName ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.BankName into g
                             select new
                             {
                                 BankName = g.Select(x => x.BankName.Trim()).FirstOrDefault()
                             };

            Index(x => x.BankName, FieldIndexing.Analyzed);
            Suggestion(x => x.BankName);
        }
    }
}

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
    public class BankBranch_Suggestions : AbstractIndexCreationTask<Employee, BankBranch_Suggestions.Projection>
    {
        public class Projection
        {
            public string BankBranch { get; set; }
        }

        public BankBranch_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    BankBranch = entity.BankBranch ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.BankBranch into g
                             select new
                             {
                                 BankBranch = g.Select(x => x.BankBranch.Trim()).FirstOrDefault()
                             };

            Index(x => x.BankBranch, FieldIndexing.Analyzed);
            Suggestion(x => x.BankBranch);
        }
    }
}

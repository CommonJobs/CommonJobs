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
    public class CompanyName_Suggestions : AbstractMultiMapIndexCreationTask<CompanyName_Suggestions.Projection>
    {
        public class Projection
        {
            public string CompanyName { get; set; }
        }

        public CompanyName_Suggestions()
        {
            AddMap<Applicant>(applicants =>
                from entity in applicants
                from item in entity.CompanyHistory
                select new
                {
                    CompanyName = item.CompanyName
                });

            Reduce = docs => from doc in docs
                             group doc by doc.CompanyName into g
                             select new
                             {
                                 CompanyName = g.Select(x => x.CompanyName.Trim()).FirstOrDefault()
                             };

            Index(x => x.CompanyName, FieldIndexing.Analyzed);
            Suggestion(x => x.CompanyName);
        }
    }
}

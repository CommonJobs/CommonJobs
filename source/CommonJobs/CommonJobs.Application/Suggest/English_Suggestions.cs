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
    public class English_Suggestions : AbstractMultiMapIndexCreationTask<English_Suggestions.Projection>
    {
        public class Projection
        {
            public string EnglishLevel { get; set; }
        }

        public English_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    EnglishLevel = entity.EnglishLevel ?? string.Empty,
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    EnglishLevel = entity.EnglishLevel ?? string.Empty,
                });

            Reduce = docs => from doc in docs
                             group doc by doc.EnglishLevel into g
                             select new
                             {
                                 EnglishLevel = g.Select(x => x.EnglishLevel.Trim()).FirstOrDefault()
                             };

            Index(x => x.EnglishLevel, FieldIndexing.Analyzed);
            Suggestion(x => x.EnglishLevel);
        }
    }
}

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
    public class Email_Suggestions : AbstractMultiMapIndexCreationTask<Email_Suggestions.Projection>
    {
        public class Projection
        {
            public string Email { get; set; }
        }

        public Email_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    Email = entity.Email ?? string.Empty,
                });

            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    Email = entity.CorporativeEmail ?? string.Empty,
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    Email = entity.Email ?? string.Empty,
                });

            Reduce = docs => from doc in docs
                             group doc by doc.Email into g
                             select new
                             {
                                 Email = g.Select(x => x.Email.Trim()).FirstOrDefault()
                             };

            Index(x => x.Email, FieldIndexing.Analyzed);
            Suggestion(x => x.Email);
        }
    }
}

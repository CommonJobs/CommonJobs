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
            public string Email { get; set; }
            public string EmailDomain { get; set; }
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
                    Email = entity.Email,
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? null : entity.Email.Split(new[] { '@' }, 2)[1],
                    EntityType = "Employee",
                    Id = entity.Id
                });

            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = entity.College,
                    Email = entity.CorporativeEmail,
                    EmailDomain = entity.CorporativeEmail == null || !entity.CorporativeEmail.Contains("@") ? null : entity.CorporativeEmail.Split(new[] { '@' }, 2)[1],
                    EntityType = "Employee",
                    Id = entity.Id
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    College = entity.College,
                    Email = entity.Email,
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? null : entity.Email.Split(new[] { '@' }, 2)[1],
                    EntityType = "Applicant",
                    Id = entity.Id
                });

            Reduce = docs => from doc in docs
                             group doc by new { doc.Id, doc.Email } into g
                             select new
                             {
                                College = g.Select(x => x.College).FirstOrDefault(),
                                Email = g.Select(x => x.Email).FirstOrDefault(),
                                EmailDomain = g.Select(x => x.EmailDomain).FirstOrDefault(),
                                EntityType = g.Select(x => x.EntityType).FirstOrDefault(),
                                Id = g.Select(x => x.Id).FirstOrDefault()
                             };

            Index(x => x.College, FieldIndexing.Analyzed);
            Index(x => x.Email, FieldIndexing.NotAnalyzed);
            Index(x => x.EmailDomain, FieldIndexing.NotAnalyzed);
        }
    }
}

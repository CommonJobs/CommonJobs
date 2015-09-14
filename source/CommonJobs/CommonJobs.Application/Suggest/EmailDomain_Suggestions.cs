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
    public class EmailDomain_Suggestions : AbstractMultiMapIndexCreationTask<EmailDomain_Suggestions.Projection>
    {
        public class Projection
        {
            public string EmailDomain { get; set; }
        }

        public EmailDomain_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? string.Empty : entity.Email.Split(new[] { '@' }, 2)[1] ?? string.Empty,
                });

            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    EmailDomain = entity.CorporativeEmail == null || !entity.CorporativeEmail.Contains("@") ? string.Empty : entity.CorporativeEmail.Split(new[] { '@' }, 2)[1] ?? string.Empty,
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? string.Empty : entity.Email.Split(new[] { '@' }, 2)[1] ?? string.Empty,
                });

            Reduce = docs => from doc in docs
                             group doc by doc.EmailDomain into g
                             select new
                             {
                                 EmailDomain = g.Select(x => x.EmailDomain.Trim()).FirstOrDefault()
                             };

            Index(x => x.EmailDomain, FieldIndexing.Analyzed);
            Suggestion(x => x.EmailDomain);
        }
    }
}

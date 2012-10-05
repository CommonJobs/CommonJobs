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
            public string EnglishLevel { get; set; }
            public string Degree { get; set; }
            public string Email { get; set; }
            public string EmailDomain { get; set; }
            public string EntityType { get; set; }
            public string Id { get; set; }
            public string BankName { get; set; }
            public string Seniority { get; set; }
            public string Platform { get; set; }
            public string Project { get; set; }
            public string Agreement { get; set; }
            public string Position  { get; set; }
        }

        public Persons_Suggestions()
        {
            //Main employees indexer
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = entity.College,
                    EnglishLevel = entity.EnglishLevel,
                    Degree = entity.Degree,
                    Email = entity.Email,
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? null : entity.Email.Split(new[] { '@' }, 2)[1],
                    EntityType = "Employee",
                    Id = entity.Id,
                    BankName = entity.BankName,
                    Seniority = entity.Seniority,
                    Platform = entity.Platform,
                    Project = entity.CurrentProject,
                    Agreement = entity.Agreement,
                    Position = entity.InitialPosition
                });

            //Secondary employees indexer (Corporative email and CurrentPosition)
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = (string)null,
                    EnglishLevel = (string)null,
                    Degree = (string)null,
                    Email = entity.CorporativeEmail,
                    EmailDomain = entity.CorporativeEmail == null || !entity.CorporativeEmail.Contains("@") ? null : entity.CorporativeEmail.Split(new[] { '@' }, 2)[1],
                    EntityType = "Employee",
                    Id = entity.Id,
                    BankName = (string)null,
                    Seniority = (string)null,
                    Platform = (string)null,
                    Project = (string)null,
                    Agreement = (string)null,
                    Position = entity.CurrentPosition
                });

            //Main applicants indexer
            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    College = entity.College,
                    EnglishLevel = entity.EnglishLevel,
                    Degree = entity.Degree,
                    Email = entity.Email,
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? null : entity.Email.Split(new[] { '@' }, 2)[1],
                    EntityType = "Applicant",
                    Id = entity.Id,
                    BankName = (string)null,
                    Seniority = (string)null,
                    Platform = (string)null,
                    Project = (string)null,
                    Agreement = (string)null,
                    Position = (string)null
                });

            Reduce = docs => from doc in docs
                             group doc by new 
                             { 
                                 doc.Id, 
                                 doc.Email,
                                 doc.Position
                             } into g
                             select new
                             {
                                College = g.Select(x => x.College).FirstOrDefault(),
                                EnglishLevel = g.Select(x => x.EnglishLevel).FirstOrDefault(),
                                Degree = g.Select(x => x.Degree).FirstOrDefault(),
                                Email = g.Select(x => x.Email).FirstOrDefault(),
                                EmailDomain = g.Select(x => x.EmailDomain).FirstOrDefault(),
                                EntityType = g.Select(x => x.EntityType).FirstOrDefault(),
                                Id = g.Select(x => x.Id).FirstOrDefault(),
                                BankName = g.Select(x => x.BankName).FirstOrDefault(),
                                Seniority = g.Select(x => x.Seniority).FirstOrDefault(),
                                Platform = g.Select(x => x.Platform).FirstOrDefault(),
                                Project = g.Select(x => x.Project).FirstOrDefault(),
                                Agreement = g.Select(x => x.Agreement).FirstOrDefault(),
                                Position = g.Select(x => x.Position).FirstOrDefault()
                             };

            Index(x => x.College, FieldIndexing.Analyzed);
            Index(x => x.EnglishLevel, FieldIndexing.NotAnalyzed);
            Index(x => x.Degree, FieldIndexing.NotAnalyzed);
            Index(x => x.Email, FieldIndexing.NotAnalyzed);
            Index(x => x.EmailDomain, FieldIndexing.NotAnalyzed);
            Index(x => x.BankName, FieldIndexing.Analyzed);
            Index(x => x.Seniority, FieldIndexing.Analyzed);
            Index(x => x.Platform, FieldIndexing.Analyzed);
            Index(x => x.Project, FieldIndexing.Analyzed);
            Index(x => x.Agreement, FieldIndexing.Analyzed);
            Index(x => x.Position, FieldIndexing.Analyzed);
        }
    }
}

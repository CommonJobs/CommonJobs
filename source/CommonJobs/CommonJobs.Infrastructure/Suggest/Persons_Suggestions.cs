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
            public string HealthInsurance { get; set; }
            public string BankBranch { get; set; }
            public string Seniority { get; set; }
            public string Platform { get; set; }
            public string Project { get; set; }
            public string Agreement { get; set; }
            public string Position { get; set; }
            public string Skill { get; set; }
            public string CompanyName { get; set; }
        }

        public Persons_Suggestions()
        {
            //Main employees indexer
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = entity.College ?? string.Empty,
                    EnglishLevel = entity.EnglishLevel ?? string.Empty,
                    Degree = entity.Degree ?? string.Empty,
                    Email = entity.Email ?? string.Empty,
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? string.Empty : entity.Email.Split(new[] { '@' }, 2)[1] ?? string.Empty,
                    EntityType = "Employee",
                    Id = entity.Id,
                    BankName = entity.BankName ?? string.Empty,
                    HealthInsurance = entity.HealthInsurance ?? string.Empty,
                    BankBranch = entity.BankBranch ?? string.Empty,
                    Seniority = entity.Seniority ?? string.Empty,
                    Platform = entity.Platform ?? string.Empty,
                    Project = entity.CurrentProject ?? string.Empty,
                    Agreement = entity.Agreement ?? string.Empty,
                    Position = entity.InitialPosition ?? string.Empty,
                    Skill = string.Empty,
                    CompanyName = string.Empty
                });

            //Secondary employees indexer (Corporative email and CurrentPosition)
            AddMap<Employee>(employees =>
                from entity in employees
                select new
                {
                    College = string.Empty,
                    EnglishLevel = string.Empty,
                    Degree = string.Empty,
                    Email = entity.CorporativeEmail,
                    EmailDomain = entity.CorporativeEmail == null || !entity.CorporativeEmail.Contains("@") ? string.Empty : entity.CorporativeEmail.Split(new[] { '@' }, 2)[1] ?? string.Empty,
                    EntityType = "Employee",
                    Id = entity.Id,
                    BankName = string.Empty,
                    HealthInsurance = string.Empty,
                    BankBranch = string.Empty,
                    Seniority = string.Empty,
                    Platform = string.Empty,
                    Project = string.Empty,
                    Agreement = string.Empty,
                    Position = entity.CurrentPosition,
                    Skill = string.Empty,
                    CompanyName = string.Empty
                });

            //Third employees indexer (Skills)
            AddMap<Employee>(employees =>
                from entity in employees
                from skill in entity.Skills.Split(new[] { '-', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                select new
                {
                    College = string.Empty,
                    EnglishLevel = string.Empty,
                    Degree = string.Empty,
                    Email = string.Empty,
                    EmailDomain = string.Empty,
                    EntityType = "Employee",
                    Id = entity.Id,
                    BankName = string.Empty,
                    HealthInsurance = string.Empty,
                    BankBranch = string.Empty,
                    Seniority = string.Empty,
                    Platform = string.Empty,
                    Project = string.Empty,
                    Agreement = string.Empty,
                    Position = string.Empty,
                    Skill = skill,
                    CompanyName = string.Empty
                });

            //Main applicants indexer
            AddMap<Applicant>(applicants =>
                from entity in applicants
                select new
                {
                    College = entity.College ?? string.Empty,
                    EnglishLevel = entity.EnglishLevel ?? string.Empty,
                    Degree = entity.Degree ?? string.Empty,
                    Email = entity.Email ?? string.Empty,
                    EmailDomain = entity.Email == null || !entity.Email.Contains("@") ? string.Empty : entity.Email.Split(new[] { '@' }, 2)[1] ?? string.Empty,
                    EntityType = "Applicant",
                    Id = entity.Id,
                    BankName = string.Empty,
                    HealthInsurance = string.Empty,
                    BankBranch = string.Empty,
                    Seniority = string.Empty,
                    Platform = string.Empty,
                    Project = string.Empty,
                    Agreement = string.Empty,
                    Position = string.Empty,
                    Skill = string.Empty,
                    CompanyName = string.Empty
                });

            //Secondary applicants indexer (CompanyNames)
            AddMap<Applicant>(applicants =>
                from entity in applicants
                from item in entity.CompanyHistory
                select new
                {
                    College = string.Empty,
                    EnglishLevel = string.Empty,
                    Degree = string.Empty,
                    Email = string.Empty,
                    EmailDomain = string.Empty,
                    EntityType = "Applicant",
                    Id = entity.Id,
                    BankName = string.Empty,
                    HealthInsurance = string.Empty,
                    BankBranch = string.Empty,
                    Seniority = string.Empty,
                    Platform = string.Empty,
                    Project = string.Empty,
                    Agreement = string.Empty,
                    Position = string.Empty,
                    Skill = string.Empty,
                    CompanyName = item.CompanyName
                });
            
            //Third applicants indexer (Skills)
            AddMap<Applicant>(applicant =>
                from entity in applicant
                from skill in entity.Skills.Split(new[] { '-', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                select new
                {
                    College = string.Empty,
                    EnglishLevel = string.Empty,
                    Degree = string.Empty,
                    Email = string.Empty,
                    EmailDomain = string.Empty,
                    EntityType = "Applicant",
                    Id = entity.Id,
                    BankName = string.Empty,
                    HealthInsurance = string.Empty,
                    BankBranch = string.Empty,
                    Seniority = string.Empty,
                    Platform = string.Empty,
                    Project = string.Empty,
                    Agreement = string.Empty,
                    Position = string.Empty,
                    Skill = skill,
                    CompanyName = string.Empty
                });

            Reduce = docs => from doc in docs
                             group doc by new 
                             { 
                                 doc.Id, 
                                 doc.Email,
                                 doc.Position,
                                 doc.Skill,
                                 doc.CompanyName
                             } into g
                             select new
                             {
                                 College = g.Select(x => x.College.Trim()).FirstOrDefault(),
                                 EnglishLevel = g.Select(x => x.EnglishLevel.Trim()).FirstOrDefault(),
                                 Degree = g.Select(x => x.Degree.Trim()).FirstOrDefault(),
                                 Email = g.Select(x => x.Email.Trim()).FirstOrDefault(),
                                 EmailDomain = g.Select(x => x.EmailDomain.Trim()).FirstOrDefault(),
                                 EntityType = g.Select(x => x.EntityType.Trim()).FirstOrDefault(),
                                 Id = g.Select(x => x.Id).FirstOrDefault(),
                                 BankName = g.Select(x => x.BankName.Trim()).FirstOrDefault(),
                                 BankBranch = g.Select(x => x.BankBranch.Trim()).FirstOrDefault(),
                                 HealthInsurance = g.Select(x => x.HealthInsurance.Trim()).FirstOrDefault(),
                                 Seniority = g.Select(x => x.Seniority.Trim()).FirstOrDefault(),
                                 Platform = g.Select(x => x.Platform.Trim()).FirstOrDefault(),
                                 Project = g.Select(x => x.Project.Trim()).FirstOrDefault(),
                                 Agreement = g.Select(x => x.Agreement.Trim()).FirstOrDefault(),
                                 Position = g.Select(x => x.Position.Trim()).FirstOrDefault(),
                                 Skill = g.Select(x => x.Skill.Trim()).FirstOrDefault(),
                                 CompanyName = g.Select(x => x.CompanyName.Trim()).FirstOrDefault()
                             };

            Index(x => x.College, FieldIndexing.Analyzed);
            Index(x => x.EnglishLevel, FieldIndexing.Analyzed);
            Index(x => x.Degree, FieldIndexing.Analyzed);
            Index(x => x.Email, FieldIndexing.Analyzed);
            Index(x => x.EmailDomain, FieldIndexing.Analyzed);
            Index(x => x.BankName, FieldIndexing.Analyzed);
            Index(x => x.HealthInsurance, FieldIndexing.Analyzed);
            Index(x => x.BankBranch, FieldIndexing.Analyzed);
            Index(x => x.Seniority, FieldIndexing.Analyzed);
            Index(x => x.Platform, FieldIndexing.Analyzed);
            Index(x => x.Project, FieldIndexing.Analyzed);
            Index(x => x.Agreement, FieldIndexing.Analyzed);
            Index(x => x.Position, FieldIndexing.Analyzed);
            Index(x => x.Skill, FieldIndexing.Analyzed);
            Index(x => x.CompanyName, FieldIndexing.Analyzed);
        }
    }
}

using CommonJobs.Domain;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.JobSearchSearching
{
    public class Applicant_BySkills : AbstractIndexCreationTask<Applicant, Applicant_BySkills.Projection>
    {
        public class Projection
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Searcheables { get; set; }
            public TechnicalSkill[] TechnicalSkills { get; set; }
            public int Total { get; set; }
        }

        public Applicant_BySkills()
        {
            Map = applicants => from applicant in applicants
                                from skill in applicant.TechnicalSkills
                                select new
                                {
                                    Id = applicant.Id,
                                    FirstName = applicant.FirstName,
                                    LastName = applicant.LastName,
                                    Searcheables = new object[] { skill.Searcheable },
                                    TechnicalSkills = new object[] { skill },
                                    Total = skill.Weight
                                };
            
            Reduce = doc => doc
                .GroupBy(x => x.Id)
                .Select(g => new {
                    Id = g.Key,
                    FirstName = g.FirstOrDefault().FirstName,
                    LastName = g.FirstOrDefault().LastName,
                    Searcheables = g.SelectMany(x => x.Searcheables).Distinct().ToArray(),
                    TechnicalSkills = g.SelectMany(x => x.TechnicalSkills).Distinct().ToArray(),
                    Total = g.Sum(x => x.Total)
                });
        }
    }
}

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
            public List<TechnicalSkill> TechnicalSkills { get; set; }
            public int Total { get; set; }
        }

        public Applicant_BySkills()
        {
            Map = applicants => from applicant in applicants
                                select new
                                {
                                    Id = applicant.Id,
                                    FirstName = applicant.FirstName,
                                    LastName = applicant.LastName,
                                    Searcheables = applicant.TechnicalSkills.Select(x => x.Searcheable).ToArray(),
                                    TechnicalSkills = applicant.TechnicalSkills,
                                    Total = applicant.TechnicalSkills == null ? 0 : applicant.TechnicalSkills.Sum(ts => ts.Weight)
                                };
            //TODO fix reduce? Total still is returned as 0 everytime
            Reduce = doc => doc
                .GroupBy(x => x.Id)
                .Select(g => new {
                    Id = g.Key,
                    FirstName = g.FirstOrDefault().FirstName,
                    LastName = g.FirstOrDefault().LastName,
                    Searcheables = g.FirstOrDefault().Searcheables,
                    TechnicalSkills = g.FirstOrDefault().TechnicalSkills,
                    Total = g.FirstOrDefault().Total
                });
        }
    }
}

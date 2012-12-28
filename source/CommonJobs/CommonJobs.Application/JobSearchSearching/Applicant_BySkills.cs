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
                                    Total = applicant.TechnicalSkills == null ? 0 : applicant.TechnicalSkills.Sum(ts => ts.Weight)
                                };
        }
    }
}

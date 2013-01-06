using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.JobSearchSearching
{
    public class GetJobSearchSuggestedApplicants : Query<GetJobSearchSuggestedApplicants.Projection[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        public JobSearch JobSearch { get; set; }

        public class Projection
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Id { get; set; }
            public List<TechnicalSkill> TechnicalSkills { get; set; }
            public int Total { get; set; }
        }

        public GetJobSearchSuggestedApplicants(JobSearch search)
        {
            this.JobSearch = search;
        }

        public override Projection[] Execute()
        {
            RavenQueryStatistics stats;

            var query = RavenSession
                .Query<Applicant_BySkills.Projection, Applicant_BySkills>();

            if (JobSearch.RequiredTechnicalSkills != null) 
                foreach (var rts in JobSearch.RequiredTechnicalSkills)
                    query = query.Where(a => a.Searcheables.StartsWith(rts.Searcheable));

            query = query
                .OrderByDescending(a => a.Total)
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            var result = query.AsProjection<Projection>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

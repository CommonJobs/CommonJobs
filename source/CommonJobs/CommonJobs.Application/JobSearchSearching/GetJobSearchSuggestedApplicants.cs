using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.JobSearchSearching
{
    public class GetJobSearchSuggestedApplicants: Query<Applicant[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        public JobSearch JobSearch { get; set; }

        public GetJobSearchSuggestedApplicants(JobSearch search)
        {
            this.JobSearch = search;
        }

        public override Applicant[] Execute()
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

            var result = query.As<Applicant>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

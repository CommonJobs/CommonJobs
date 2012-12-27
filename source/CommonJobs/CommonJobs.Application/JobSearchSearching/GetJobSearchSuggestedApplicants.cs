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
                .Query<Applicant>();

            if (JobSearch.RequiredTechnicalSkills != null) 
                foreach (var rts in JobSearch.RequiredTechnicalSkills)
                    query = query.Where(a => a.TechnicalSkills.Any(ts => ts.Name == rts.Name && ts.Level > rts.Level));

            query = query
                //.OrderByDescending(a => a.TechnicalSkills.Sum(ts => (int)ts.Level))*/
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            var result = query.ToArray();
            Stats = stats;
            return result;
        }
    }
}

using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.JobSearchSearching
{
    public class GetJobSearchSuggestedApplicants : Query<SuggestedApplicantsResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        public TechnicalSkill[] RequiredTechnicalSkills { get; set; }

        public GetJobSearchSuggestedApplicants(TechnicalSkill[] requiredTechnicalSkills)
        {
            this.RequiredTechnicalSkills = requiredTechnicalSkills;
        }

        public override SuggestedApplicantsResult[] Execute()
        {
            RavenQueryStatistics stats;

            var query = RavenSession
                .Query<Applicant_BySkills.Projection, Applicant_BySkills>();

            if (RequiredTechnicalSkills != null) 
                foreach (var rts in RequiredTechnicalSkills)
                    query = query.Where(a => a.Searcheables.StartsWith(rts.Searcheable));

            query = query
                .OrderByDescending(a => a.Total)
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            var result = query.AsProjection<SuggestedApplicantsResult>().ToArray();
            Stats = stats;
            return result;
        }
    }
}

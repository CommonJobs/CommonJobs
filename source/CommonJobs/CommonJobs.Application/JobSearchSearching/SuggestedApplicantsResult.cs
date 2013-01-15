using CommonJobs.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.JobSearchSearching
{
    public class SuggestedApplicantsResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public TechnicalSkill[] TechnicalSkills { get; set; }
        public int Total { get; set; }
    }
}

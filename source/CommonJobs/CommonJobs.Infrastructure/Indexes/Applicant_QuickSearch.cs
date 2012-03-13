using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.Indexes
{
    public class Applicant_QuickSearch: AbstractIndexCreationTask<Applicant, Applicant_QuickSearch.Query>
    {
        public class Query
        {
            public string ByTerm { get; set; }
            public bool Highlighted { get; set; }
            public bool HaveInterview { get; set; }
            public bool HaveTechnicalInterview { get; set; }
            public string SortingField { get; set; }
        }

        public Applicant_QuickSearch()
        {
            Map = applicants => from applicant in applicants
                                select new
                                {
                                    ByTerm = new object[]
                                    {
                                        applicant.FirstName,
                                        applicant.LastName,
                                        applicant.CompanyHistory.Select(x => x.CompanyName),
                                        applicant.Skills
                                    },
                                    Highlighted = applicant.IsHighlighted,
                                    HaveInterview = applicant.HaveInterview,
                                    HaveTechnicalInterview = applicant.HaveTechnicalInterview,
                                    SortingField = string.Format("{0}, {1}", applicant.LastName, applicant.FirstName) 
                                };
            Indexes.Add(x => x.ByTerm, FieldIndexing.Analyzed);
        }
    }
}

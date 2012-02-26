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
                                        string.Join(" ", applicant.CompanyHistory.Select(x => x.CompanyName)),
                                        applicant.Skills
                                    }
                                };
            Indexes.Add(x => x.ByTerm, FieldIndexing.Analyzed);
        }
    }
}

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
                                        applicant.Id,
                                        applicant.FirstName,
                                        applicant.LastName,
                                        applicant.Address,
                                        applicant.Telephones,
                                        applicant.MaritalStatus,
                                        string.Format("{0:yyyy-MM-dd}", applicant.BirthDate),
                                        string.Format("{0:dd-MM-yyyy}", applicant.BirthDate),
                                        string.Format("{0:MM-dd-yyyy}", applicant.BirthDate),
                                        string.Format("{0:MMMM}", applicant.BirthDate),
                                        applicant.Email,
                                        applicant.College,
                                        applicant.Degree,
                                        applicant.Skills
                                    }
                                };
            Indexes.Add(x => x.ByTerm, FieldIndexing.Analyzed);
        }
    }
}

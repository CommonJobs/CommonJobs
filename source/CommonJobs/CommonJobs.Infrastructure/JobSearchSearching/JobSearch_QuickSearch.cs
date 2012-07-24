using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CommonJobs.Infrastructure.JobSearchSearching
{
    public class JobSearch_QuickSearch: AbstractMultiMapIndexCreationTask<JobSearch_QuickSearch.Projection>
    {
        public class Projection 
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public bool IsJobSearch { get; set; }
            public string PublicNotes { get; set; }
            public string PrivateNotes { get; set; }
            public bool IsPublic { get; set; }
        }

        public JobSearch_QuickSearch()
        {
            AddMap<JobSearch>(jobSearches =>
                from jobSearch in jobSearches
                select new
                {
                    Id = jobSearch.Id,
                    Title = jobSearch.Title,
                    PublicNotes = jobSearch.PublicNotes,
                    PrivateNotes = jobSearch.PrivateNotes,
                    IsPublic = jobSearch.IsPublic
                });

            Reduce = docs =>
                from doc in docs
                group doc by doc.Id into g
                select new
                {
                    Id = g.Key,
                    Title = g.FirstOrDefault().Title,
                    PublicNotes = g.FirstOrDefault().PublicNotes,
                    PrivateNotes = g.FirstOrDefault().PrivateNotes,
                    IsPublic = g.FirstOrDefault().IsPublic
                };

            Indexes.Add(x => x.Title, FieldIndexing.Analyzed);
        }
    }
}

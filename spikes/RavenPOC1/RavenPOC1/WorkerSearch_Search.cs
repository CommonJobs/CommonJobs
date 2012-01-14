using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using RavenPOC1.Domain;

namespace RavenPOC1
{
    public class WorkerSearch_Search : AbstractIndexCreationTask<WorkerSearch, WorkerSearch_Search.ReduceResult>
    {
        public class ReduceResult
        {
            public string Query { get; set; }
            public DateTime Date { get; set; }
        }

        public WorkerSearch_Search()
        {
            Map = searchs => from search in searchs
                            select new
                            {
                                Query = new object[]
                                {
                                    search.Title,
                                    search.Description,
                                    search.Skills
                                },
                                Date = search.Date
                            };
        }
    }

}

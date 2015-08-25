using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Linq;
using CommonJobs.Infrastructure.RavenDb;
using CommonJobs.Domain;
using CommonJobs.ContentExtraction;
using CommonJobs.Application.Indexes;
using System.Linq.Expressions;
using CommonJobs.Utilities;
using Raven.Client;

namespace CommonJobs.Application.SharedLinks
{
    public class SearchSharedEntity : Query<string>
    {
        public RavenQueryStatistics Stats { get; set; }
        public string SharedCode { get; set; }
        public string EntityId { get; set; }

        public SearchSharedEntity(string sharedCode, string entityId = null)
        {
            SharedCode = sharedCode;
            EntityId = entityId;
        }

        public override string Execute()
        {
            RavenQueryStatistics stats;
            var query = RavenSession
                .Query<SharedLinks_Entities.ReduceResult, SharedLinks_Entities>()
                .Statistics(out stats)
                .Where(x => x.ExpirationDate > DateTime.Now)
                .Where(x => x.SharedCode == SharedCode);

            if (EntityId != null)
                query = query.Where(x => x.EntityId == EntityId);

            var results = query
                .Select(x => x.EntityId)
                .Take(2)
                .ToArray();

            if (results.Length == 1)
                return results[0];
            else
                return null;
        }
    }
}

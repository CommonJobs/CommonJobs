using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System.Globalization;
using CommonJobs.Domain;

namespace CommonJobs.Infrastructure.SharedLinks
{
    public class SharedLinks_Entities : AbstractMultiMapIndexCreationTask<SharedLinks_Entities.ReduceResult>
    {
        public class ReduceResult
        {
            public string EntityId { get; set; }
            public string SharedCode { get; set; }
            public DateTime ExpirationDate { get; set; }
        }

        public SharedLinks_Entities()
		{
            AddMap<Applicant>(entities =>
                from entity in entities
                from sharedLink in entity.SharedLinks
                select new
                {
                    EntityId = entity.Id,
                    SharedCode = sharedLink.SharedCode,
                    ExpirationDate = sharedLink.ExpirationDate
                });

            //TODO: No estoy muy seguro de que JobSearch sea realmente un SharedEntity
            AddMap<JobSearch>(entities =>
                from entity in entities
                from sharedLink in entity.SharedLinks
                where entity.IsPublic
                select new
                {
                    EntityId = entity.Id,
                    SharedCode = sharedLink.SharedCode,
                    ExpirationDate = sharedLink.ExpirationDate
                });
		}
    }
}
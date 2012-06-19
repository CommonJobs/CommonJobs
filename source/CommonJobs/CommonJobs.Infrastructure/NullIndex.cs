using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CommonJobs.Infrastructure.Indexes
{
    public class NullIndex : AbstractIndexCreationTask<NullIndex.SampleCollection>
    {
        public class SampleCollection
        {
            public string Name { get; set; }
        }

        public NullIndex()
		{
			Map = objects => from obj in objects
						   select new { Name = obj.Name };

			Reduce = results => from r in results
                                group r by r.Name
								into g
                                select new { Name = g.Key };
		}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Application.Persons;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System.Text.RegularExpressions;
using CommonJobs.Utilities;

namespace CommonJobs.Application.ApplicantFlow
{
    public class GetEventTypesQuery : Query<EventType[]>
    {
        public override EventType[] Execute()
        {
            //It is only getting predefined EventTypes
            var query = RavenSession.Query<ApplicantEventType>()
                .OrderBy(x => x.Text);

            var results = query
                .AsEnumerable()
                .Distinct()
                .ToArray();

            return results;
        }
    }
}

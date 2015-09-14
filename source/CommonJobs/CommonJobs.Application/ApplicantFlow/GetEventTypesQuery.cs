using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using System.Text.RegularExpressions;
using CommonJobs.Utilities;
using Raven.Client;

namespace CommonJobs.Application.ApplicantFlow
{
    public class GetEventTypesQuery : Query<EventType[]>
    {
        public override EventType[] Execute()
        {
            RavenQueryStatistics stats;

            IQueryable<ApplicantEventType_Suggestions.Projection> query =
                RavenSession.Query<ApplicantEventType_Suggestions.Projection, ApplicantEventType_Suggestions>()
                .OrderByDescending(x => x.Predefined)
                .ThenBy(x => x.Text)
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            var results = query
                .As<EventType>()
                .ToArray();

            return results;
        }
    }
}

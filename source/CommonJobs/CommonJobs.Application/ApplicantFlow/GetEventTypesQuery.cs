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
            var query = RavenSession.Query<ApplicantEventType_Suggestions.Projection, ApplicantEventType_Suggestions>()
                .OrderByDescending(x => x.Predefined)
                .ThenBy(x => x.Text);

            var results = query
                .AsProjection<EventType>()
                .ToArray();

            return results;
        }
    }
}

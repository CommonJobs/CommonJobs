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

namespace CommonJobs.Application.EmployeeAbsences
{
    public class GetAbsenceReasons : Query<AbsencesReasonResult[]>
    {
        public override AbsencesReasonResult[] Execute()
        {
            var query = RavenSession.Query<EmployeeAbsences_Suggestions.Projection, EmployeeAbsences_Suggestions>()
                .OrderByDescending(x => x.Predefined)
                .ThenBy(x => x.Text);

            var results = query
                .AsEnumerable()
                .Select(x => new AbsencesReasonResult()
                {
                    Text = x.Text,
                    Color = x.Color, //TODO: set a random color?
                    Predefined = x.Predefined
                })
                .Distinct()
                .ToArray();

            return results;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Application.EmployeeAbsences
{
    public class EmployeeAbsences_Suggestions : AbstractMultiMapIndexCreationTask<EmployeeAbsences_Suggestions.Projection>
    {
        public class Projection
        {
            public string Text { get; set; }
            public string Color { get; set; }
            public bool Predefined { get; set; }
        }

        public EmployeeAbsences_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                from absence in entity.Absences
                select new
                {
                    Text = string.IsNullOrWhiteSpace(absence.Reason) ? null : absence.Reason.Trim(),
                    Color = (string)null,
                    Predefined = false
                });

            AddMap<AbsenceReason>(reasons =>
                from entity in reasons
                select new
                {
                    Text = string.IsNullOrWhiteSpace(entity.Text) ? null : entity.Text.Trim(),
                    Color = entity.Color,
                    Predefined = true
                });

            Reduce = docs => from doc in docs
                             group doc by new
                             {
                                 doc.Text
                             } into g
                             select new
                             {
                                 Text = g.OrderByDescending(x => x.Predefined).Select(x => x.Text).FirstOrDefault(),
                                 Color = g.OrderByDescending(x => x.Predefined).Select(x => x.Color).FirstOrDefault(),
                                 Predefined = g.Any(x => x.Predefined)
                             };

            Index(x => x.Text, FieldIndexing.Analyzed);
            Suggestion(x => x.Text);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.EmployeeAbsences
{
    public class EmployeeAbsences_Suggestions : AbstractMultiMapIndexCreationTask<EmployeeAbsences_Suggestions.Projection>
    {
        public class Projection
        {
            public string Text { get; set; }
            public string Color { get; set; }
        }

        public EmployeeAbsences_Suggestions()
        {
            //Main employees indexer
            AddMap<Employee>(employees =>
                from entity in employees
                from absence in entity.Absences
                select new
                {
                    Text = absence.Reason ?? string.Empty,
                    Color = string.Empty
                });

            //Secondary employees indexer (Corporative email and CurrentPosition)
            AddMap<AbsenceReason>(reasons =>
                from entity in reasons
                select new
                {
                    Text = entity.Text ?? string.Empty,
                    Color = entity.Color ?? string.Empty
                });

            Reduce = docs => from doc in docs
                             group doc by new 
                             { 
                                 doc.Text,
                                 doc.Color
                             } into g
                             select new
                             {
                                 Text = g.OrderByDescending(x => x.Color).Select(x => x.Text.Trim()).FirstOrDefault(),
                                 Color = g.OrderByDescending(x => x.Color).Select(x => x.Color.Trim()).FirstOrDefault()
                             };

            Index(x => x.Text, FieldIndexing.NotAnalyzed);
        }
    }
}

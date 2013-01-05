using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Application.ApplicantFlow
{
    public class ApplicantEventType_Suggestions : AbstractMultiMapIndexCreationTask<ApplicantEventType_Suggestions.Projection>
    {
        public class Projection
        {
            public string Text { get; set; }
            public string Slug { get; set; }
            public string Color { get; set; }
            public bool Predefined { get; set; }
        }

        public ApplicantEventType_Suggestions()
        {
            AddMap<Applicant>(applicants =>
                from entity in applicants
                from note in entity.Notes
                select new
                {
                    Text = string.IsNullOrWhiteSpace(note.EventType) ? null : note.EventType.Trim(),
                    Slug = (string)null,
                    Color = (string)null,
                    Predefined = false
                });

            AddMap<ApplicantEventType>(types =>
                from entity in types
                select new
                {
                    Text = string.IsNullOrWhiteSpace(entity.Text) ? null : entity.Text.Trim(),
                    Slug = string.IsNullOrWhiteSpace(entity.Slug) ? null : entity.Slug.Trim(),
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
                                 Slug = g.OrderByDescending(x => x.Predefined).Select(x => x.Slug).FirstOrDefault(),
                                 Color = g.OrderByDescending(x => x.Predefined).Select(x => x.Color).FirstOrDefault(),
                                 Predefined = g.Any(x => x.Predefined)
                             };

            TransformResults = (db, results) => results.Where(x => !string.IsNullOrWhiteSpace(x.Text)).Select(x => x);

            Index(x => x.Text, FieldIndexing.Analyzed);
        }
    }
}

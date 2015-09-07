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
            public string Id { get; set; }
            public string Slug { get; set; }
            public string Text { get; set; }
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
                    Id = (string)null,
                    Slug = note.EventTypeSlug,
                    Text = string.IsNullOrWhiteSpace(note.EventType) ? null : note.EventType.Trim(),
                    Color = (string)null,
                    Predefined = false
                });

            AddMap<ApplicantEventType>(types =>
                from entity in types
                select new
                {
                    Id = entity.Id,
                    Slug = entity.Slug,
                    Text = string.IsNullOrWhiteSpace(entity.Text) ? null : entity.Text.Trim(),
                    Color = entity.Color,
                    Predefined = true
                });

            Reduce = docs => from doc in docs
                             group doc by new
                             {
                                 doc.Slug
                             } into g
                             select new
                             {
                                 Id = g.OrderByDescending(x => x.Predefined).Select(x => x.Id).FirstOrDefault(),
                                 Slug = g.OrderByDescending(x => x.Predefined).Select(x => x.Slug).FirstOrDefault(),
                                 Text = g.OrderByDescending(x => x.Predefined).Select(x => x.Text).FirstOrDefault(),
                                 Color = g.OrderByDescending(x => x.Predefined).Select(x => x.Color).FirstOrDefault(),
                                 Predefined = g.Any(x => x.Predefined)
                             };

            Index(x => x.Slug, FieldIndexing.NotAnalyzed);
            Index(x => x.Text, FieldIndexing.Analyzed);
        }
    }
}

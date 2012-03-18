using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.Indexes
{
    public class Applicant_QuickSearch: AbstractMultiMapIndexCreationTask<Applicant_QuickSearch.Projection>
    {
        public class Projection
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string[] Companies { get; set; }
            public string Skills { get; set; }
            public string FullName1 { get; set; }
            public string FullName2 { get; set; }
            public bool IsHighlighted { get; set; }
            public bool HaveInterview { get; set; }
            public bool HaveTechnicalInterview { get; set; }
            public string[] AttachmentIds { get; set; }
            public string[] AttachmentNames { get; set; }
            public string[] AttachmentsContent { get; set; }
        }

        public Applicant_QuickSearch()
        {
            //TODO: index attachments too

            AddMap<Applicant>(applicants => from applicant in applicants
                                select new
                                {
                                    Id = applicant.Id,
                                    FirstName = applicant.FirstName,
                                    LastName = applicant.LastName,
                                    Companies = applicant.CompanyHistory.Select(x => x.CompanyName).ToArray(),
                                    Skills = applicant.Skills,
                                    FullName1 = string.Format("{0}, {1}", applicant.LastName, applicant.FirstName),
                                    FullName2 = string.Format("{0} {1}", applicant.FirstName, applicant.LastName),
                                    IsHighlighted = applicant.IsHighlighted,
                                    HaveInterview = applicant.HaveInterview,
                                    HaveTechnicalInterview = applicant.HaveTechnicalInterview,
                                    AttachmentIds = applicant.AllAttachmentReferences.Select(x => x.Id).ToArray(),
                                    AttachmentNames = applicant.AllAttachmentReferences.Select(x => x.FileName).ToArray(),
                                    AttachmentsContent = new string[0]
                                });

            Reduce = docs => from doc in docs
                             group doc by doc.Id into g
                             select new
                             {
                                Id = g.Key,
                                FirstName = g.Where(x => x.FirstName != null).Select(x => x.FirstName).FirstOrDefault(),
                                LastName = g.Where(x => x.LastName != null).Select(x => x.LastName).FirstOrDefault(),
                                Companies = g.SelectMany(x => x.Companies).Distinct().ToArray(),
                                Skills = g.Where(x => x.Skills != null).Select(x => x.Skills).FirstOrDefault(),
                                FullName1 = g.Where(x => x.FullName1 != null).Select(x => x.FullName1).FirstOrDefault(),
                                FullName2 = g.Where(x => x.FullName2 != null).Select(x => x.FullName2).FirstOrDefault(),
                                IsHighlighted = g.Any(x => x.IsHighlighted),
                                HaveInterview = g.Any(x => x.IsHighlighted),
                                HaveTechnicalInterview = g.Any(x => x.IsHighlighted),
                                AttachmentIds = g.SelectMany(x => x.AttachmentIds).Distinct().ToArray(),
                                AttachmentNames = g.SelectMany(x => x.AttachmentNames).Distinct().ToArray(),
                                AttachmentsContent = g.SelectMany(x => x.AttachmentsContent).Distinct().ToArray()
                             };

            Indexes.Add(x => x.FirstName, FieldIndexing.Analyzed);
            Indexes.Add(x => x.LastName, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Companies, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Skills, FieldIndexing.Analyzed);
            Indexes.Add(x => x.AttachmentNames, FieldIndexing.Analyzed);
            Indexes.Add(x => x.AttachmentsContent, FieldIndexing.Analyzed);
        }
    }
}

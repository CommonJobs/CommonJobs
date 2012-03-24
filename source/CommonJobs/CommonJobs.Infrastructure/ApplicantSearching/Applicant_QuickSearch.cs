using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.ApplicantSearching
{
    public class Applicant_QuickSearch: AbstractMultiMapIndexCreationTask<Applicant_QuickSearch.Projection>
    {
        //TODO: Mejorar esto. Está realmente feo y llena el indice con archivos repetidos, 
        //pero es la única forma de hacerlo funcionar que encontré hasta ahora

        public class OrphanAttachment
        {
            public string Id { get; set; }
            public string FileName { get; set; }
            public string PlainContent { get; set; }
        }

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
            public string[] AttachmentContent { get; set; }

            public bool IsApplicant { get; set; }
            public OrphanAttachment[] OrphanAttachments { get; set; }
        }

        public Applicant_QuickSearch()
        {
            AddMap<Applicant>(applicants => 
                from applicant in applicants
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
                    AttachmentContent = new string[0],
                    IsApplicant = true,
                    OrphanAttachments = new dynamic[0]
                });

            AddMap<Attachment>(attachments =>
                from attachment in attachments
                select new
                {
                    Id = attachment.RelatedEntityId,
                    FirstName = (string)null,
                    LastName = (string)null,
                    Companies = new string[0],
                    Skills = (string)null,
                    FullName1 = (string)null,
                    FullName2 = (string)null,
                    IsHighlighted = false,
                    HaveInterview = false,
                    HaveTechnicalInterview = false,
                    AttachmentIds = new string[0],
                    AttachmentNames = new string[0],
                    AttachmentContent = new string[0],
                    IsApplicant = false,
                    OrphanAttachments = new[] { new { Id = attachment.Id, FileName = attachment.FileName, PlainContent = attachment.PlainContent } },
                });

            Reduce = docs => 
                from doc in docs
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

                    IsApplicant = g.Any(x => x.IsApplicant),

                    AttachmentContent = g.SelectMany(x => x.AttachmentContent).Union(
                        g.SelectMany(x => x.OrphanAttachments).Where(x => g.SelectMany(y => y.AttachmentIds).Contains(x.Id)).Select(x => x.PlainContent)
                    ).ToArray(),
                    OrphanAttachments = g.SelectMany(x => x.OrphanAttachments).Where(x => !g.SelectMany(y => y.AttachmentIds).Contains(x.Id)).ToArray()
                };

            Indexes.Add(x => x.FirstName, FieldIndexing.Analyzed);
            Indexes.Add(x => x.LastName, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Companies, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Skills, FieldIndexing.Analyzed);
            Indexes.Add(x => x.AttachmentNames, FieldIndexing.Analyzed);
            Indexes.Add(x => x.AttachmentContent, FieldIndexing.Analyzed);
        }
    }
}

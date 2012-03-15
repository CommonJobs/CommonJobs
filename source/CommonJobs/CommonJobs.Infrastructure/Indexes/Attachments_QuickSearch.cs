using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Indexes;
using CommonJobs.Domain;
using Raven.Abstractions.Indexing;

namespace CommonJobs.Infrastructure.Indexes
{
    public class Attachments_QuickSearch : AbstractMultiMapIndexCreationTask<Attachments_QuickSearch.Result>
    {
        //TODO: permitir indexar coleccion de key/values para metadatos (Por ejemplo los de los archivos de word)

        public class Result
        {
            public string AttachmentId { get; set; }
            public string PlainContent { get; set; }
            public string ContentType { get; set; }
            public string FileName { get; set; }
            public string RelatedEntityId { get; set; }
            public bool IsOrphan { get; set; }
            public string ContentExtractorConfigurationHash { get; set; }
        }

        public Attachments_QuickSearch()
        {
            AddMap<Attachment>(attachments =>
                from attachment in attachments
                select new
                {
                    AttachmentId = attachment.Id,
                    PlainContent = attachment.PlainContent,
                    ContentType = attachment.ContentType,
                    FileName = attachment.FileName,
                    RelatedEntityId = attachment.RelatedEntityId,
                    ContentExtractorConfigurationHash = attachment.ContentExtractorConfigurationHash,
                    IsOrphan = true
                });

            //TODO: hacer esto para cualquier entidad que tenga la propiedad AllAttachmentReferences
            AddMap<Employee>(employees =>
                from entity in employees
                from attachmentReference in entity.AllAttachmentReferences
                select new
                {
                    AttachmentId = attachmentReference.Id,
                    PlainContent = (string)null,
                    ContentType = (string)null,
                    FileName = (string)null,
                    RelatedEntityId = (string)null,
                    ContentExtractorConfigurationHash = (string)null,
                    IsOrphan = false
                });

            AddMap<Applicant>(applicants =>
                from entity in applicants
                from attachmentReference in entity.AllAttachmentReferences
                select new
                {
                    AttachmentId = attachmentReference.Id,
                    PlainContent = (string)null,
                    ContentType = (string)null,
                    FileName = (string)null,
                    RelatedEntityId = (string)null,
                    ContentExtractorConfigurationHash = (string)null,
                    IsOrphan = false
                });

            Reduce = docs => from doc in docs
                             group doc by doc.AttachmentId into g
                             select new
                             {
                                AttachmentId = g.Key,
                                PlainContent = g.Select(x => x.PlainContent).Where(x => x != null).FirstOrDefault(),
                                ContentType = g.Select(x => x.ContentType).Where(x => x != null).FirstOrDefault(),
                                FileName = g.Select(x => x.FileName).Where(x => x != null).FirstOrDefault(),
                                RelatedEntityId = g.Select(x => x.RelatedEntityId).Where(x => x != null).FirstOrDefault(),
                                ContentExtractorConfigurationHash = g.Select(x => x.ContentExtractorConfigurationHash).Where(x => x != null).FirstOrDefault(),
                                IsOrphan = g.Select(x => x.IsOrphan).All(x => x)
                             };

            Index(x => x.PlainContent, FieldIndexing.Analyzed);
            //TODO: permitir busquedas por nombre de archivos con comodines
            Index(x => x.FileName, FieldIndexing.Analyzed);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Infrastructure;
using Raven.Client.Linq;
using CommonJobs.Domain;
using System.Linq.Expressions;
using CommonJobs.Utilities;

namespace CommonJobs.Infrastructure.AttachmentSearching
{
    public class SearchAttachments : Query<AttachmentSearchResult[]>
    {
        public RavenQueryStatistics Stats { get; set; }
        AttachmentSearchParameters Parameters { get; set; }

        public SearchAttachments(AttachmentSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override AttachmentSearchResult[] Execute()
        {
            //TODO: agregar soporte para filtrar por tipo de archivos o con comodines en el nombre 
            RavenQueryStatistics stats;
            var query = RavenSession.Query<Attachments_QuickSearch.Projection, Attachments_QuickSearch>()
                .Statistics(out stats)
                .Include<Attachments_QuickSearch.Projection>(x => x.RelatedEntityId) //Con esto el Load no necesita consultar la DB
                .ApplyPagination(Parameters);

            if (!string.IsNullOrWhiteSpace(Parameters.Term))
            {
                //TODO Soportar comodines o algún tipo de expressión
                Expression<Func<Attachments_QuickSearch.Projection, bool>> predicate = x =>
                    x.FileName.StartsWith(Parameters.Term);

                if (!Parameters.SearchOnlyInFileName)
                    predicate = predicate.Or(x => x.FullText.Any(y => y.StartsWith(Parameters.Term)));

                query = query.Where(predicate);
            }
            /*
            if (!string.IsNullOrWhiteSpace(Parameters.Term))
                ;
             * 
             SearchOnlyInFileName
             * 
             if (Parameters.SearchInAttachments)
                predicate.Or(x => x.AttachmentContent.Any(y => y.StartsWith(Parameters.Term)));
            */


            if (!Parameters.IncludeFilesWithoutText)
                query = query.Where(x => x.HasText);

            if (Parameters.Orphans == OrphansMode.NoOrphans)
                query = query.Where(x => !x.IsOrphan);
            else if (Parameters.Orphans == OrphansMode.OnlyOrphans)
                query = query.Where(x => x.IsOrphan);

            var aux = query
                //Projection in order to do not bring PlainContent (big field)
                .Select(x => new
                {
                    x.AttachmentId,
                    x.ContentType,
                    x.FileName,
                    x.RelatedEntityId,
                    x.IsOrphan,
                    x.PartialText
                })
                .ToArray();

            var results = aux
                .Select(x => new AttachmentSearchResult()
                {
                    AttachmentId = x.AttachmentId,
                    ContentType = x.ContentType,
                    FileName = x.FileName,
                    IsOrphan = x.IsOrphan,
                    PartialText = x.PartialText,
                    RelatedEntity = x.IsOrphan || string.IsNullOrEmpty(x.RelatedEntityId) ? null : RavenSession.Load<Person>(x.RelatedEntityId)
                })
                .ToArray();
                
            Stats = stats;
            return results;
        }
    }
}

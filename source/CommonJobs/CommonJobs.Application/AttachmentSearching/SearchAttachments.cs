using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client.Linq;
using CommonJobs.Domain;
using System.Linq.Expressions;
using CommonJobs.Utilities;
using Raven.Client;

namespace CommonJobs.Application.AttachmentSearching
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
            var term = string.IsNullOrWhiteSpace(Parameters.Term) ? "*" : Parameters.Term.Trim();
            RavenQueryStatistics stats;

            var query = RavenSession
                .Query<Attachments_QuickSearch.Projection, Attachments_QuickSearch>()
                .Statistics(out stats)
                .Include<Attachments_QuickSearch.Projection>(x => x.RelatedEntityId); //Con esto el Load no necesita consultar la DB

            var nameTerm = term.TrimEnd(new[] { '*', '?' }) + '*';

            //Hack porque FieldIndexing.Default hace algo raro cuando busco con espacios, hay que investigarlo mas
            query = query.Search(x => x.FileNameWithoutSpaces, nameTerm.Replace(" ", "·"), escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);

            if (!Parameters.SearchOnlyInFileName)
            {
                var fullTextTerm = term.Trim(new[] { '*', '?' }) + '*';
                query = query.Search(x => x.FullText, fullTextTerm, escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard);
            }

            if (!Parameters.IncludeFilesWithoutText)
            {
                //Hack porque
                //  ravenQuery = ravenQuery.Search(x => x.HasText, true.ToString(), options: SearchOptions.And);
                //debería generar
                //  FileNameWithoutSpaces:<<*english.pdf*>> AND HasText:<<True>>
                //y genera
                //  FileNameWithoutSpaces:<<*english.pdf*>> HasText:<<True>>
                query = query.Search(x => x.HasText, false.ToString(), options: SearchOptions.And | SearchOptions.Not);
            }

            if (Parameters.Orphans == OrphansMode.NoOrphans)
            {
                query = query.Search(x => x.IsOrphan, true.ToString(), options: SearchOptions.And | SearchOptions.Not);
            }
            else if (Parameters.Orphans == OrphansMode.OnlyOrphans)
            {
                query = query.Search(x => x.IsOrphan, false.ToString(), options: SearchOptions.And | SearchOptions.Not);
            }

            var qArray = query.ApplyPagination(Parameters).ToArray();
            var aux = qArray.Select(x => new
                {
                    x.AttachmentId,
                    x.ContentType,
                    x.FileName,
                    x.RelatedEntityId,
                    x.IsOrphan,
                    x.PartialText
                });

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

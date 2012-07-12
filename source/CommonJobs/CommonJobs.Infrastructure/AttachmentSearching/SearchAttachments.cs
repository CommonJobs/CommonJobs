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
            RavenQueryStatistics stats;
            var baseQuery = RavenSession.Query<Attachments_QuickSearch.Projection, Attachments_QuickSearch>();

            var ravenQuery = baseQuery;

            if (!string.IsNullOrWhiteSpace(Parameters.Term))
            {
                ravenQuery = ravenQuery.Search(x => x.FileNameWithoutSpaces, Parameters.Term.Replace(" ", string.Empty), escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards);
                
                if (!Parameters.SearchOnlyInFileName)
                    ravenQuery = ravenQuery.Search(x => x.FullText, Parameters.Term, escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard);
                        //.Or(x => x.FullText.Any(y => y.StartsWith(Parameters.Term)));

                //ravenQuery = ravenQuery.Where(predicate);
            }

            //TODO: 
            // * similatr behaviour to startwith in FullText search
            // * Add Orphans and HasText filters

            /*
            if (!Parameters.IncludeFilesWithoutText)
            {
                var a = baseQuery.Where(x => x.HasText);
                var c = ravenQuery.Intersect(a);

                ravenQuery = ravenQuery.Intersect(baseQuery.Where(x => x.HasText));
            }

            if (Parameters.Orphans == OrphansMode.NoOrphans)
            {
                ravenQuery = ravenQuery.Intersect(baseQuery.Where(x => !x.IsOrphan));
            }
            else if (Parameters.Orphans == OrphansMode.OnlyOrphans)
            {
                ravenQuery = ravenQuery.Intersect(baseQuery.Where(x => x.IsOrphan));
            }
            */

            var query = 
                ravenQuery
                .Statistics(out stats)
                .Include<Attachments_QuickSearch.Projection>(x => x.RelatedEntityId) //Con esto el Load no necesita consultar la DB
                .ApplyPagination(Parameters);
            
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

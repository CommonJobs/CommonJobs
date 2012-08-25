using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using System.Linq.Expressions;

namespace CommonJobs.Raven.Migrations
{
    public abstract class Migration : CommonJobs.Raven.Migrations.IMigration
    {
        public IDocumentStore DocumentStore { get; set; }

        public abstract void Up();
        public abstract void Down();

        protected bool IndexExists(string index, int pageSize = 64)
        {
            int start = 0;
            while (true)
            {
                var names = DocumentStore.DatabaseCommands.GetIndexNames(start, pageSize);
                if (names.Length == 0)
                    return false;
                else if (names.Contains(index))
                    return true;
                start += pageSize;
            }
        }

        protected void DoInResults(
            Action<RavenJObject> action, 
            string query = "*:*", 
            int pageSize = 64, 
            string index = null, 
            string[] includes = null,
            string[] fieldsToFetch = null,
            string sortedBy = null
            )
        {
            int start = 0;
            while (true)
            {
                var qry = new IndexQuery() 
                { 
                    Query = query, 
                    PageSize = pageSize,
                    Start = start //TODO: consider to use another thing in place of start
                };

                if (fieldsToFetch != null)
                {
                    qry.FieldsToFetch = fieldsToFetch;
                }

                if (sortedBy != null)
                {
                    qry.SortedFields = new SortedField[] { new SortedField(sortedBy) };
                }

                if (index != null)
                {
                    CheckStale(index);
                }

                var results = DocumentStore.DatabaseCommands.Query(index ?? "Dynamic", qry, includes);

                if (results.Results.Count == 0)
                    break;

                foreach (var result in results.Results)
                    action(result);

                start += pageSize;
            }
        }

        public void CheckStale(string index, int timeOut = 3000)
        {
            if (DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Contains(index))
            {
                System.Threading.Thread.Sleep(timeOut);
                if (DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Contains(index))
                {
                    throw new ApplicationException(string.Format("Index {0} is stale after waiting for {1} ms.", index, timeOut));
                }
            }
        }

        public void ReSaveAll<TEntity, TKey>(Expression<Func<TEntity, TKey>> orderByExpr, int size = 10)
        {
            var current = 0;
            TEntity[] entities;
            do
            {
                using (var session = DocumentStore.OpenSession())
                {
                    entities = session.Query<TEntity>().OrderBy(orderByExpr).Skip(current).Take(size).ToArray();
                    current += size;
                    foreach (var entity in entities)
                    {
                        session.Store(entity);
                    }
                    session.SaveChanges();
                }
            }
            while (entities.Length > 0);
        }
    }
}

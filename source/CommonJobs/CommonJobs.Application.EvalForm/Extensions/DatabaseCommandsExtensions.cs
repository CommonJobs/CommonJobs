using Raven.Abstractions.Data;
using Raven.Client.Connection;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Extensions
{
    public static class DatabaseCommandsExtensions
    {
        public static Operation DeleteByIndex<T>(
           this IDatabaseCommands databaseCommands,
           IndexQuery queryToDelete,
           BulkOperationOptions options = null)
           where T : AbstractIndexCreationTask
        {
            var indexName = typeof(T).Name.Replace("_", "/");
            return databaseCommands.DeleteByIndex(indexName, queryToDelete, options);
        }
    }
}

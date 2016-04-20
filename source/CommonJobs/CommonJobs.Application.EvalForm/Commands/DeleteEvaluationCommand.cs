using CommonJobs.Application.EvalForm.Indexes;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Abstractions.Data;
using CommonJobs.Application.EvalForm.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class DeleteEvaluationCommand : Command<bool>
    {

        public DeleteEvaluationCommand(string period, string username = null)
        {
            _userName = username;
            _period = period;
        }

        public string _userName { get; set; }
        public string _period { get; set; }

        public override bool ExecuteWithResult()
        {
            var indexQuery = new IndexQuery
            {
                Query = "Period:" + Escape(_period)
            };
            if (_userName != null)
            {
                indexQuery.Query = indexQuery.Query + "AND UserName:" + Escape(_userName);
            }

            RavenSession.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex<EvaluationToDelete_Search>(indexQuery).WaitForCompletion();
            return true;
        }

        /// <summary>
        /// Escapes symbols like * or " to avoid deleting more data than intended
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Escape(string value)
        {
            return "\"" + value.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }
    }
}

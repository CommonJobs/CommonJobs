using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Newtonsoft.Json;

namespace CommonJobs.Infrastructure.RavenDb
{
    public abstract class Command
    {
        [JsonIgnore]
        public IDocumentSession RavenSession { get; set; }
        public abstract void Execute();

        protected void ExecuteCommand(Command cmd)
        {
            cmd.RavenSession = RavenSession;
            cmd.Execute();
        }

        protected TResult ExecuteCommand<TResult>(Command<TResult> cmd)
        {
            cmd.RavenSession = RavenSession;
            cmd.Execute();
            return cmd.Result;
        }

        protected TResult Query<TResult>(Query<TResult> qry)
        {
            qry.RavenSession = RavenSession;
            return qry.Execute();
        }
    }

    public abstract class Command<TResult> : Command
    {
        public TResult Result { get; protected set; }

        public override void Execute()
        {
            Result = ExecuteWithResult();
        }

        public abstract TResult ExecuteWithResult();
    }

}
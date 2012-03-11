using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;

namespace CommonJobs.Mvc
{
    public abstract class Query<TResult>
    {
        public IDocumentSession RavenSession { get; set; }
        public abstract TResult Execute();
    }

}
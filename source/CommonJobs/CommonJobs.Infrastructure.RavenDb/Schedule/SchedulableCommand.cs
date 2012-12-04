using Newtonsoft.Json;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.RavenDb.Schedule
{
    public abstract class SchedulableCommand : Command
    {
        [JsonIgnore]
        public Func<DateTime> Now { get; set; }
        
        public DateTime ExecuteAndGetNext(IDocumentSession ravenSession, Func<DateTime> now, DateTime scheduled)
        {
            RavenSession = ravenSession;
            Now = now;
            var start = Now();
            if (IsExecutionRequired())
                Execute();
            return CalculateNextExecutionTime(start, scheduled);
        }
        
        protected abstract DateTime CalculateNextExecutionTime(DateTime start, DateTime scheduled);

        /// <summary>
        /// Verify if it is really necesary to execute the command, because something could change from the last schedule
        /// </summary>
        protected abstract bool IsExecutionRequired();
    }
}

using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.RavenDb.Schedule
{
    public class ScheduleTask
    {
        public string Id { get; set; }
        public DateTime LastExecution { get; set; }
        public DateTime NextExecution { get; set; }
        public bool IsActive { get; set; }
        public SchedulableCommand Command { get; set; }

        public void Execute(IDocumentSession ravenSession, Func<DateTime> now)
        {
            LastExecution = now();
            var nextExecution = Command.ExecuteAndGetNext(ravenSession, now, NextExecution);
            IsActive = nextExecution < DateTime.MaxValue;
            NextExecution = nextExecution;
            ravenSession.SaveChanges();
        }
    }
}

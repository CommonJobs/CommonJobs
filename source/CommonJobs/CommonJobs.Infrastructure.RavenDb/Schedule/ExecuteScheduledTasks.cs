using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;
using Raven.Client;

namespace CommonJobs.Infrastructure.RavenDb.Schedule
{
    public class ExecuteScheduledTasks : Command
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        public Func<DateTime> Now { get; set; }

        public ExecuteScheduledTasks()
        {
            Now = () => DateTime.Now;
        }

        public override void Execute()
        {
            var now = Now();
            var tasks = RavenSession.Query<ScheduleTask>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                .Where(x => x.IsActive)
                .Where(x => x.NextExecution <= now)
                .Take(255) //TODO: it is not scalable
                .ToArray();

            foreach (var task in tasks)
            {
                try
                {
                    task.Execute(RavenSession, Now);
                }
                catch (Exception e)
                {
                    log.ErrorException("Error executing schedule task", e);
                    log.Dump(LogLevel.Error, task);
                }
            }
        }
    }
}

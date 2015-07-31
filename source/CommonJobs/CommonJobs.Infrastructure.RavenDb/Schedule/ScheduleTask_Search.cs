using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.RavenDb.Schedule
{
    public class ScheduleTask_Search : AbstractIndexCreationTask<ScheduleTask>
    {
        public ScheduleTask_Search()
        {
            Map = m => m.Select(e => new
            {
                e.IsActive,
                e.NextExecution
            });
        }
    }
}

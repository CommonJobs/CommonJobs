using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Migrations
{
    public class MigrationAction
    {
        public string Id { get; set; }
        public MigrationActionType Action { get; set; }
    }
}

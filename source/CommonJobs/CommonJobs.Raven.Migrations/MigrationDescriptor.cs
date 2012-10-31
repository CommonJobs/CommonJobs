using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace CommonJobs.Infrastructure.Migrations
{
    public class MigrationDescriptor 
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string MigrationTypeFullName { get; set; }
        public MigrationStatus Status { get; set; }
        public List<MigrationMessage> Messages { get; set; }

        public MigrationDescriptor()
        {
            Messages = new List<MigrationMessage>();
        }
    }
}

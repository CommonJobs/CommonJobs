using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace CommonJobs.Raven.Migrations
{
    public class MigrationStatus
    {
        public MigrationDescriptor MigrationDescriptor { get; set; }
        public bool Installed { get; set; }
        public bool Obsolete { get; set; } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace CommonJobs.Raven.Migrations
{
    public abstract class Migration : CommonJobs.Raven.Migrations.IMigration
    {
        public IDocumentStore DocumentStore { get; set; }

        public abstract void Up();
        public abstract void Down();
    }
}

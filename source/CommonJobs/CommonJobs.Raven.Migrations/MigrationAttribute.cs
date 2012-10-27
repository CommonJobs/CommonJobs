using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace CommonJobs.Infrastructure.Migrations
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MigrationAttribute : Attribute
    {
        public string Id { get; private set; }
        public string Description { get; private set; }

        public MigrationAttribute(string id)
        {
            Id = id;
        }

        public MigrationAttribute(string id, string description) : this(id)
        {
            Description = description;
        }
    }
}

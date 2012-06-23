using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace CommonJobs.Raven.Migrations
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MigrationAttribute : Attribute
    {
        public string Id 
        {
            get { return ObsoleteId ? Key : "MigrationDescriptors/" + Key; }
        }

        public string Key { get; private set; }
        public string Description { get; private set; }
        public bool ObsoleteId { get; set; }

        public MigrationAttribute(string key)
        {
            Key = key;
        }

        public MigrationAttribute(string key, string description) : this(key)
        {
            Description = description;
        }
    }
}

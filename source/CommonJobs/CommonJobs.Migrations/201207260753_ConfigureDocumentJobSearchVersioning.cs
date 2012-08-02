using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    [Migration("201207260753", "Configure Versioning Bundle for JobSearchs")]
    public class ConfigureDocumentJobSearchVersioning : Migration
    {
        public override void Up()
        {
            using (var session = DocumentStore.OpenSession())
            {
                DeleteConfiguration(session, "Raven/Versioning/JobSearchs");
                session.SaveChanges();
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/JobSearchs",
                });
                session.SaveChanges();
            }
        }

        public override void Down()
        {
            using (var session = DocumentStore.OpenSession())
            {
                DeleteConfiguration(session, "Raven/Versioning/JobSearchs");
                session.SaveChanges();
            }
        }

        private static void DeleteConfiguration(IDocumentSession session, string key)
        {
            var applicantsConfiguration = session.Load<dynamic>(key);
            if (applicantsConfiguration != null)
                session.Delete(applicantsConfiguration);
        }
    }
}

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
    [Migration("201207210850", "Configure Versioning Bundle")]
    public class ConfigureDocumentVersioning : Migration
    {
        public override void Up()
        {
            using (var session = DocumentStore.OpenSession())
            {
                DeleteConfiguration(session, "Raven/Versioning/DefaultConfiguration");
                DeleteConfiguration(session, "Raven/Versioning/Applicants");
                DeleteConfiguration(session, "Raven/Versioning/Employees");
                session.SaveChanges();
                session.Store(new
                {
                    Exclude = true,
                    Id = "Raven/Versioning/DefaultConfiguration"
                });
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/Applicants",
                });
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/Employees",
                });
                session.SaveChanges();
            }
        }

        public override void Down()
        {
            using (var session = DocumentStore.OpenSession())
            {
                DeleteConfiguration(session, "Raven/Versioning/DefaultConfiguration");
                DeleteConfiguration(session, "Raven/Versioning/Applicants");
                DeleteConfiguration(session, "Raven/Versioning/Employees");
                session.SaveChanges();
                session.Store(new
                {
                    Exclude = true,
                    Id = "Raven/Versioning/DefaultConfiguration"
                });
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

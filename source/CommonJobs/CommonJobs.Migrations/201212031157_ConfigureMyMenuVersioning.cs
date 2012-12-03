using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    [Migration("201212031157", "Configure Versioning Bundle for MyMenu documents")]
    public class ConfigureMyMenuVersioning : Migration
    {
        public override void Up()
        {
            using (var session = DocumentStore.OpenSession())
            {
                DeleteConfiguration(session, "Raven/Versioning/EmployeeMenus");
                DeleteConfiguration(session, "Raven/Versioning/MenuOrders");
                DeleteConfiguration(session, "Raven/Versioning/DailyMenuRequests");
                DeleteConfiguration(session, "Raven/Versioning/Menus");
                session.SaveChanges();
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/EmployeeMenus",
                });
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/MenuOrders",
                });
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/DailyMenuRequests",
                });
                session.Store(new
                {
                    Exclude = false,
                    Id = "Raven/Versioning/Menus",
                });
                session.SaveChanges();
            }
        }

        public override void Down()
        {
            using (var session = DocumentStore.OpenSession())
            {
                DeleteConfiguration(session, "Raven/Versioning/EmployeeMenus");
                DeleteConfiguration(session, "Raven/Versioning/MenuOrders");
                DeleteConfiguration(session, "Raven/Versioning/DailyMenuRequests");
                DeleteConfiguration(session, "Raven/Versioning/Menus");
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

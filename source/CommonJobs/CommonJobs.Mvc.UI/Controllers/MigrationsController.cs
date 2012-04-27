using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Raven.Migrations;
using CommonJobs.Raven.Mvc;
using CommonJobs.Migrations;
using Miscellaneous.Attributes.Controller;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class MigrationsController : CommonJobsController
    {
        //
        // GET: /Migrations
        [FilterIP(
            ConfigurationKeyAllowedMaskedIPs="AllowedMigrationMaskedIPs", 
            ConfigurationKeyAllowedSingleIPs="AllowedMigrationSingleIPs",
            ConfigurationKeyDeniedMaskedIPs="DeniedMigrationMaskedIPs",
            ConfigurationKeyDeniedSingleIPs="DeniedMigrationSingleIPs")]
        public ActionResult Index(string id)
        {
            var migrator = new Migrator(RavenSessionManager.DocumentStore, typeof(MigrationClass).Assembly);
            var migrations = migrator.GetMigrationStatus();

            return View();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.Migrations;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Migrations;
using Miscellaneous.Attributes.Controller;
using CommonJobs.Mvc.UI.Infrastructure;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles="Migrators")]
    [Documentation("manual-de-usuario/administracion/Migraciones")]
    public class MigrationsController : Controller
    {
        //
        // GET: /Migrations
        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("Index")]
        public ActionResult Get(string id)
        {
            var migrator = new Migrator(RavenSessionManager.DocumentStore, typeof(MigrationClass).Assembly);
            var status = migrator.GetMigrationStatus();

            return View(status);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("Index")]
        [ValidateAntiForgeryToken(Salt = "To avoid IP spoofing")] //http://stackoverflow.com/questions/8170830/can-request-userhostaddress-tostring-be-spoofed-asp-net-4-0-iis-7-5
        public ActionResult Post(IEnumerable<MigrationAction> actions)
        {
            var migrator = new Migrator(RavenSessionManager.DocumentStore, typeof(MigrationClass).Assembly);
            migrator.RunActions(actions);
            return RedirectToAction("Index");
        }
    }
}

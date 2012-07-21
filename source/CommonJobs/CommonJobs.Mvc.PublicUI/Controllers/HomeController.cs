using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure.AttachmentStorage;
using CommonJobs.Raven.Mvc;

namespace CommonJobs.Mvc.PublicUI.Controllers
{
    public class HomeController : CommonJobsController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Attach(string id)
        {
            var entity = RavenSession.Load<object>(id);
            if (entity == null)
                return HttpNotFound("Specified entity does not exists");

            var attachmentReader = new RequestAttachmentReader(Request);
            var attachment = ExecuteCommand(new SaveAttachment(
                entity,
                attachmentReader.FileName,
                attachmentReader.Stream));

            return Json(new { success = true, attachment = attachment });
        }
    }
}

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

            using (var attachmentReader = new RequestAttachmentReader(Request))
            {
                if (attachmentReader.Count != 1)
                    throw new NotSupportedException("One and only one attachment is required.");

                var attachment = attachmentReader.First();

                var result = ExecuteCommand(new SaveAttachment(
                    entity,
                    attachment.Key,
                    attachment.Value));

                return Json(new { success = true, attachment = result });
            }
        }
    }
}

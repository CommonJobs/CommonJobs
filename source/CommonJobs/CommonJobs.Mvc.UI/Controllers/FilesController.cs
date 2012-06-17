using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Raven.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    [Authorize]
    public class FilesController : CommonJobsController
    {
        public ActionResult Index(string searchTerm)
        {
            return View(searchTerm);
        }

        public JsonResult List(string searchTerm)
        {
            //TODO: real search
            var results = new List<Attachment>() {
                new Attachment("employees/1", "test1.pdf", "000001"),
                new Attachment("employees/1", "test1.png", "000002"),
                new Attachment("employees/1", "test1.gif", "000003"),
                new Attachment("employees/2", "test1.docx", "000004"),
                new Attachment("employees/2", "test1.xls", "000005"),
                new Attachment("employees/2", "test1.txt", "000006"),
                new Attachment("employees/2", "test1.jpg", "000007"),
            };

            return Json(new
            {
                Items = results,
                //Skipped = ...
                //TotalResults = ...
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

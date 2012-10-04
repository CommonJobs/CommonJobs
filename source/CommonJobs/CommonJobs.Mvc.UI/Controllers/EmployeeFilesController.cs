using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Infrastructure;
using CommonJobs.Infrastructure.EmployeeFiles;
using CommonJobs.Raven.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    //NOTE: "Employee Files" as a name refers to the file that is the source of record for employee data
    //It is NOT meant as an synonym to "attachment"
    [CommonJobsAuthorize(Roles = "Users")]
    public class EmployeeFilesController : CommonJobsController
    {
        public ActionResult Index(int batchSize = 100)
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData", new  {
                    batchSize = batchSize
                }
            );

            return View();
        }

        public JsonNetResult EmployeeFileBatch(BaseSearchParameters parameters)
        {
            var query = new SearchEmployeeFiles(parameters);
            var results = Query(query);
            return Json(new
            {
                Items = results,
                Skipped = parameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        }
    }
}

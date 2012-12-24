using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Application;
using CommonJobs.Application.EmployeeFiles;
using CommonJobs.Infrastructure.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    //NOTE: "Employee Files" as a name refers to the file that is the source of record for employee data
    //It is NOT meant as an synonym to "attachment"
    [CommonJobsAuthorize(Roles = "Users,EmployeeManagers")]
    public class EmployeeFilesController : CommonJobsController
    {
        public ActionResult Index(int bsize = 10)
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData", new  {
                    bsize = bsize
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

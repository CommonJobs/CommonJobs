using CommonJobs.Application.Evaluations;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Mvc.UI.Areas.Evaluations.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Areas.Evaluations.Controllers
{
    public class EvaluationsApiController : CommonJobsController
    {

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetEmployeesToGenerateEvalution(string period)
        {
            //1. Check if period exists by querying Evaluations collection.
            //2. Check if the current user has to evaluate someone
            //2.a. If not, redirect to /Evaluations/{period}/{username}
            //2.b. If so, fetch the users to evaluate

            PeriodCreation periodCreation = new PeriodCreation();
            periodCreation.Employees = ExecuteCommand(new GetEmployeesForEvaluationCommand(period));
            return Json(periodCreation);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult GenerateEvalutions(PeriodCreation model, string period)
        {
            ExecuteCommand(new GenerateEvaluationsCommand(model.Employees, period));
            return Json(model.Employees.Count);
        }
    }
}

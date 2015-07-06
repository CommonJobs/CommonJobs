using CommonJobs.Application.EvalForm;
using CommonJobs.Application.EvalForm.Commands;
using CommonJobs.Application.EvalForm.Dtos;
using CommonJobs.Application.Evaluations;
using CommonJobs.Domain;
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
        private string DetectUser()
        {
            //TODO: remove hardcoded "CS\\"
            //TODO: move to an AuthorizeAttribute or something more elegant
            if (User != null && User.Identity != null && User.Identity.Name != null)
            {
                var user = User.Identity.Name;
                if (user.StartsWith("CS\\"))
                    user = user.Substring(3);
                return user;
            }
            else
            {
                throw new ApplicationException("User cannot be detected");
            }
        }

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
        public JsonNetResult GenerateEvalutions(PeriodCreation model)
        {
            ExecuteCommand(new GenerateEvaluationsCommand(model.Employees));
            return Json(model.Employees.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetDashboardEvaluations(string period) {
            PeriodEvaluation periodEvaluation = new PeriodEvaluation();
            periodEvaluation.Evaluations = ExecuteCommand(new GetEvaluatorEmployeesCommand(DetectUser()));
            return Json(periodEvaluation);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetCalificatorsForEvaluation(string username)
        {
            PeriodEvaluation periodEvaluation = new PeriodEvaluation();
            periodEvaluation.Evaluations = ExecuteCommand(new GetEvaluatorEmployeesCommand(username));
            return Json(periodEvaluation);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult UpdateCalificators(EmployeeEvaluationDTO evaluation, List<EvaluatorsUpdateDto> calificators)
        {
            ExecuteCommand(new UpdateEvaluatorsCommand(evaluation, calificators));
            return Json("ok");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetEvaluation (string username, string period)
        {
            Calification calification = new Calification();
            CalificationsDto calificationsDTO = ExecuteCommand(new GetEvaluationCalifications(period, username, DetectUser()));
            calification.UserView = calificationsDTO.View;
            calification.Evaluation = calificationsDTO.Evaluation;
            calification.Califications = calificationsDTO.Califications;
            calification.UserLogged = DetectUser();
            calification.Template = ExecuteCommand(new GetEvaluationTemplateCommand());
            return Json(calification);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult SaveEvaluationCalifications(UpdateEvaluationDto updateEvaluationDto)
        {
            ExecuteCommand(new UpdateCalificationsCommand(updateEvaluationDto, DetectUser()));
            return Json("ok");
        }
    }
}

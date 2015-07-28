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
        [CommonJobsAuthorize(Roles = "EmployeeManagers")]
        public JsonNetResult GetEmployeesToGenerateEvalution(string period)
        {
            PeriodCreation periodCreation = new PeriodCreation();
            periodCreation.Employees = ExecuteCommand(new GetEmployeesForEvaluationCommand(period));
            return Json(periodCreation);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [CommonJobsAuthorize(Roles = "EmployeeManagers")]
        public JsonNetResult GenerateEvalutions(PeriodCreation model)
        {
            ExecuteCommand(new GenerateEvaluationsCommand(model.Employees));
            return Json(model.Employees.Count);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonNetResult GetDashboardEvaluationsForEmployeeManagers(string period)
        {
            PeriodEvaluation periodEvaluation = new PeriodEvaluation();
            var command = new GetEvaluatedEmployees();
            if (period != null)
            {
                command.Period = period;
            }
            periodEvaluation.Evaluations = ExecuteCommand(command);
            return Json(periodEvaluation);
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
            var sessionRoles = (string[])HttpContext.Session[CommonJobs.Mvc.UI.Controllers.AccountController.SessionRolesKey] ?? new string[] { };
            var required = new List<string>() { "EmployeeManagers" };
            var isManager = sessionRoles.Intersect(required).Any();

            Calification calification = new Calification();
            CalificationsDto calificationsDTO = ExecuteCommand(new GetEvaluationCalifications(period, username, DetectUser(), isManager));
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

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonNetResult StartDevolution(string evaluationId)
        {
            ExecuteCommand(new StartDevolutionCommand(evaluationId, DetectUser()));
            return Json("OK");
        }
    }
}

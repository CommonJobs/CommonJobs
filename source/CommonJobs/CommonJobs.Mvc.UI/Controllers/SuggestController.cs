using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Application.Suggest;
using CommonJobs.Infrastructure.Mvc;
using Raven.Abstractions.Data;
using Raven.Client.Linq;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class SuggestController : CommonJobsController
    {
        public JsonNetResult College(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetCollegeSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult EnglishLevel(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetEnglishSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Degree(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetDegreeSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users,EmployeeManagers")]
        public JsonNetResult Email(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetEmailSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult EmailDomain(string term, int maxSuggestions = 8)
        {
            if (term == null || !term.Contains("@"))
                return null; //TODO

            var parts = term.Split(new[] { '@' }, 2);
            var domainTerm = parts[1];
            var prefix = parts[0];

            var results = Query(new GetEmailDomainSuggestions(domainTerm, maxSuggestions))
                .Select(x => string.Format("{0}@{1}", prefix, x))
                .ToArray();
            return Json(new { suggestions = results });
        }

        public JsonNetResult BankName(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetBankNameSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult BankBranch(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetBankBranchSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult HealthInsurance(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetHealthInsuranceSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Seniority(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSenioritySuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Platform(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetPlatformSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users,EmployeeManagers")]
        public JsonNetResult Project(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetProjectSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users,EmployeeManagers")]
        public JsonNetResult Agreement(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetAgreementSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Position(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetPositionSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Skill(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSkillSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult TechnicalSkillName(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetTechnicalSkillSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult CompanyName(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetCompanyNameSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult AbsenceReason(string term, int maxSuggestions = 8)
        {
            var results = Query(new CommonJobs.Application.EmployeeAbsences.GetSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult UserName(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetUserNameSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult ApplicantEventType(string term, int maxSuggestions = 8)
        {
            var results = Query(new CommonJobs.Application.ApplicantFlow.GetEventTypeSuggestionsQuery(term, maxSuggestions));
            return Json(new { suggestions = results });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Application.Persons;
using CommonJobs.Application.Suggestions;
using CommonJobs.Infrastructure.Mvc;
using Raven.Abstractions.Data;
using Raven.Client.Linq;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class SuggestController : CommonJobsController
    {
        public JsonNetResult College(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.College, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult EnglishLevel(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.EnglishLevel, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Degree(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Degree, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users,EmployeesManagers")]
        public JsonNetResult Email(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Email, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult EmailDomain(string term, int maxSuggestions = 8)
        {
            if (term == null || !term.Contains("@"))
                return null; //TODO

            var parts = term.Split(new[] { '@' }, 2);
            var domainTerm = parts[1];
            var prefix = parts[0];

            var results = Query(new GetSuggestions(x => x.EmailDomain, domainTerm, maxSuggestions))
                .Select(x => string.Format("{0}@{1}", prefix, x))
                .ToArray();
            return Json(new { suggestions = results });
        }

        public JsonNetResult BankName(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.BankName, term, maxSuggestions));
            return Json(new { suggestions = results });
        }
        
        public JsonNetResult BankBranch(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.BankBranch, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult HealthInsurance(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.HealthInsurance, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Seniority(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Seniority, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Platform(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Platform, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users,EmployeesManagers")]
        public JsonNetResult Project(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Project, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users,EmployeesManagers")]
        public JsonNetResult Agreement(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Agreement, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Position(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Position, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Skill(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.Skill, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult CompanyName(string term, int maxSuggestions = 8)
        {
            var results = Query(new GetSuggestions(x => x.CompanyName, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult AbsenceReason(string term, int maxSuggestions = 8)
        {
            var results = Query(new CommonJobs.Application.EmployeeAbsences.GetSuggestions(term, maxSuggestions));
            return Json(new { suggestions = results });
        }
    }
}

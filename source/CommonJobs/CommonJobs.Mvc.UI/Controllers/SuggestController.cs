using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.Persons;
using CommonJobs.Infrastructure.Suggestions;
using CommonJobs.Raven.Mvc;
using Raven.Abstractions.Data;
using Raven.Client.Linq;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class SuggestController : CommonJobsController
    {
        public JsonNetResult College(string term, int maxSuggestions = 10)
        {
            var results = Query(new GetSuggestions(x => x.College, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult EnglishLevel(string term, int maxSuggestions = 10)
        {
            var results = Query(new GetSuggestions(x => x.EnglishLevel, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult Degree(string term, int maxSuggestions = 10)
        {
            var results = Query(new GetSuggestions(x => x.Degree, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        [CommonJobsAuthorize(Roles = "Users")]
        public JsonNetResult Email(string term, int maxSuggestions = 10)
        {
            var results = Query(new GetSuggestions(x => x.Email, term, maxSuggestions));
            return Json(new { suggestions = results });
        }

        public JsonNetResult EmailDomain(string term, int maxSuggestions = 10)
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
    }
}

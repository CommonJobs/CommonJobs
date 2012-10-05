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
    }
}

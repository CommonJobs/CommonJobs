using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.JobSearchSearching;
using CommonJobs.Raven.Mvc;
using CommonJobs.Raven.Infrastructure;

namespace CommonJobs.Mvc.UI.Controllers
{
    [CommonJobsAuthorize(Roles = "Users")]
    public class JobSearchesController : CommonJobsController
    {
        public ActionResult Index()
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    publicSiteUrl = ConfigurationManager.AppSettings["CommonJobs/PublicSiteUrl"]
                },
                500);

            return View();
        }

        //TODO Search parameters
        public JsonNetResult List(JobSearchSearchParameters searchParameters)
        {
            var query = new SearchJobSearches(searchParameters);
            var results = Query(query);
            return Json(new
            {
                Items = results,
                Skiped = searchParameters.Skip,
                TotalResults = query.Stats.TotalResults
            });
        }

        public ActionResult Create()
        {
            var newJobSearch = new JobSearch()
            {
                //TODO: maybe NewJobSearchDefaultPrefix name is not appropiated now.
                PublicCode = ConfigurationManager.AppSettings["CommonJobs/NewJobSearchDefaultPrefix"]
            };
            RavenSession.Store(newJobSearch);
            return RedirectToAction("Edit", new { id = newJobSearch.Id });
        }

        public ActionResult Edit(string id)
        {
            var jobSearch = RavenSession.Load<JobSearch>(id);
            if (jobSearch == null)
                return HttpNotFound(); 
            
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    jobSearch = jobSearch,
                    //TODO: verify this:
                    publicSiteUrl = ConfigurationManager.AppSettings["CommonJobs/PublicSiteUrl"] + "new/" + RavenSession.ExtractNumericIdentityPart(jobSearch)
                },
                500);
            
            return View();
        }

        public JsonNetResult Get(string id)
        {
            var jobSearch = RavenSession.Load<JobSearch>(id);
            return Json(jobSearch);
        }

        public JsonNetResult Post(JobSearch jobSearch)
        {
            RavenSession.Store(jobSearch);
            return Get(jobSearch.Id);
        }

        public ActionResult Delete(string id)
        {
            var jobSearch = RavenSession.Load<JobSearch>(id);
            RavenSession.Delete(jobSearch);
            return RedirectToAction("Index");
        }
    }
}

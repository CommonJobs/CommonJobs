using CommonJobs.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.PublicUI.Models
{
    public class PostulationCreationViewModel
    {
        public PostulationCreationViewModel(JobSearch jobSearch, string[] technicalSkillLevelNames)
        {
            this.JobSearchId = jobSearch.Id;
            this.JobSearchTitle = jobSearch.Title;

            var md = new MarkdownDeep.Markdown();
            this.PublicNotes = new MvcHtmlString(md.Transform(jobSearch.PublicNotes));

            this.TechnicalSkillLevelNames = technicalSkillLevelNames;
        }

        public string JobSearchId { get; set; }
        public string JobSearchTitle { get; set; }
        public MvcHtmlString PublicNotes { get; set; }
        public string[] TechnicalSkillLevelNames { get; set; }

        public Postulation InputPostulation { get; set; }
    }
}
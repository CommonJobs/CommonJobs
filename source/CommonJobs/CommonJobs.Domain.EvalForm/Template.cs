using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    public class Template
    {
        public string Id { get; set; }
        

        public List<TemplateItem> Items { get; set; }
        public List<KeyValuePair<string, string>> Groups { get; set; }

        public Template()
        {
            Items = new List<TemplateItem>();
            Groups = new List<KeyValuePair<string, string>>();
        }

        public const string DefaultTemplateId = "Template/Default";
        public const string Template2019Test = "Template/2019Test";
    }
}

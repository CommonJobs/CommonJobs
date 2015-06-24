using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    public class Template
    {
        public string Id
        {
            //TODO: Change this when support for multiple templates is added
            get { return DefaultTemplateId; }
        }

        public List<TemplateItem> Items { get; set; }

        public Template()
        {
            Items = new List<TemplateItem>();
        }

        public const string DefaultTemplateId = "Template/Default";
    }
}

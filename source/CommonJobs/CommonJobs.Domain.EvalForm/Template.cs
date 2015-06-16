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

        public Template()
        {
            Items = new List<TemplateItem>();
        }
    }
}

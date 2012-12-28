using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class ApplicantEventType
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }

        private ApplicantEventType()
        {
            //RavenDB usage
        }

        public ApplicantEventType(string text, string color)
        {
            Text = text;
            Color = color;
        }
    }
}

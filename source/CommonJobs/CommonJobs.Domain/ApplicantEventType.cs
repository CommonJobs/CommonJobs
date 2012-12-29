using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class ApplicantEventType
    {
        public const string DefaultRHInterview = "Entrevista RRHH";
        public const string DefaultTechnicalInterview = "Entrevista Técnica";
        public const string DefaultEnglishInterview = "Entrevista Inglés";
        public const string DefaultPMInterview = "Entrevista PM";
        public const string DefaultPsychologicalTest = "Test Psicológico";

        public string Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }

        public string Slug { get { return Text.GenerateSlug(); } }

        private ApplicantEventType()
        {
            //RavenDB usage
        }

        public ApplicantEventType(string text, string color)
        {
            Text = text;
            Color = color;
        }

        public bool Match(string value)
        {
            return Match(Text, value);
        }

        public static bool Match(string value1, string value2)
        {
            return value1.GenerateSlug() == value2.GenerateSlug();
        }
    }
}

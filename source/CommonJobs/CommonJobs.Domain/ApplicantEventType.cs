using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class ApplicantEventType : EventType
    {
        public const string DefaultHRInterview = "Entrevista RRHH";
        public const string DefaultTechnicalInterview = "Entrevista Técnica";
        public const string DefaultEnglishInterview = "Entrevista Inglés";
        public const string DefaultPMInterview = "Entrevista PM";
        public const string DefaultPsychologicalTest = "Test Psicológico";

        //En lugar de esta clase vacía, otra opción es hacer algo parecido a lo de los slots

        protected ApplicantEventType() : base()
        {
            //RavenDB usage
        }

        public ApplicantEventType(string text, string color)
            : base(text, color)
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    [Migration("201212280848", "Add default applicant event types configuration")]
    public class DefaultApplicantEventTypes : Migration
    {
        const string ColorRed = "#CB4D4D";
        const string ColorYellow = "#DBDB57";
        const string ColorBlue = "#4D77CB";
        const string ColorPurple = "#93C";
        const string ColorDarkGray = "#102030";
        const string ColorLightGray = "#A0B0C0";
        const string ColorOrange = "#E09952";
        const string ColorGreen = "#34B27D";
        const string ColorBrown = "#B27D34";


        private ApplicantEventType[] newOnes = new[] {
            new ApplicantEventType(ApplicantEventType.DefaultRHInterview, ColorDarkGray) { Id = "ApplicantEventTypes/entrevista-rrhh" },
            new ApplicantEventType(ApplicantEventType.DefaultTechnicalInterview,  ColorYellow) { Id = "ApplicantEventTypes/entrevista-tecnica" }, 
            new ApplicantEventType(ApplicantEventType.DefaultEnglishInterview,  ColorBlue) { Id = "ApplicantEventTypes/entrevista-ingles" },
            new ApplicantEventType(ApplicantEventType.DefaultPMInterview,  ColorOrange) { Id = "ApplicantEventTypes/entrevista-pm" },
            new ApplicantEventType(ApplicantEventType.DefaultPsychologicalTest, ColorPurple) { Id = "ApplicantEventTypes/test-psicologico" }
        };
        
        public override void Up()
        {
            Down();
            using (var session = DocumentStore.OpenSession())
            {
                var existingOnes = new HashSet<string>(LoadExisting(session).Select(x => x.Id));
                var unexistingOnes = newOnes.Where(x => !existingOnes.Contains(x.Id));

                foreach(var item in unexistingOnes)
                {
                    session.Store(item);
                }

                session.SaveChanges();
            }
        }

        public override void Down()
        {
            using (var session = DocumentStore.OpenSession())
            {
                IEnumerable<ApplicantEventType> existingOnes = LoadExisting(session);

                foreach (var item in existingOnes)
                {
                    session.Delete(item);
                }

                session.SaveChanges();
            }
        }

        private IEnumerable<ApplicantEventType> LoadExisting(IDocumentSession session)
        {
            IEnumerable<string> ids = newOnes.Select(x => x.Id);
            ApplicantEventType[] reasons = session.Load<ApplicantEventType>(ids);
            return reasons.Where(x => x != null);
        }
    }
}

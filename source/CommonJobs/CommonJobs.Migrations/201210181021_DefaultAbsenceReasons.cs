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
    [Migration("201210181021", "Add default absence reasons configuration")]
    public class DefaultAbsenceReasons : Migration
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
        

        private AbsenceReason[] newAbsenceReasons = new[] {
            new AbsenceReason("Enfermedad", ColorRed) { Id = "AbsenceReasons/enfermedad" },
            new AbsenceReason("Examen",  ColorYellow) { Id = "AbsenceReasons/examen" }, 
            new AbsenceReason("Maternidad", ColorBlue) { Id = "AbsenceReasons/maternidad" },
            new AbsenceReason("Paternidad", ColorPurple) { Id = "AbsenceReasons/paternidad" },
            new AbsenceReason("Fallecimiento de familiar", ColorDarkGray) { Id = "AbsenceReasons/fallecimiento-de-familiar" },
            new AbsenceReason("Casamiento", ColorLightGray) { Id = "AbsenceReasons/casamiento" },
            new AbsenceReason("Trámites Personales", ColorOrange) { Id = "AbsenceReasons/tramites-personales" },
            new AbsenceReason("Trámites de la empresa", ColorGreen) { Id = "AbsenceReasons/tramites-de-la-empresa" },
            new AbsenceReason("Cuestiones Personales", ColorBrown) { Id = "AbsenceReasons/cuestiones-personales" }
        };
        
        public override void Up()
        {
            Down();
            using (var session = DocumentStore.OpenSession())
            {
                var existingSlotIds = new HashSet<string>(LoadExistingReasons(session).Select(x => x.Id));
                var unexistingSlots = newAbsenceReasons.Where(x => !existingSlotIds.Contains(x.Id));

                foreach(var slot in unexistingSlots)
                {
                    session.Store(slot);
                }

                session.SaveChanges();
            }
        }

        public override void Down()
        {
            using (var session = DocumentStore.OpenSession())
            {
                IEnumerable<AbsenceReason> existingSlots = LoadExistingReasons(session);

                foreach (var slot in existingSlots)
                {
                    session.Delete(slot);
                }

                session.SaveChanges();
            }
        }

        private IEnumerable<AbsenceReason> LoadExistingReasons(IDocumentSession session)
        {
            IEnumerable<string> ids = newAbsenceReasons.Select(x => x.Id);
            AbsenceReason[] reasons = session.Load<AbsenceReason>(ids);
            return reasons.Where(x => x != null);
        }
    }
}

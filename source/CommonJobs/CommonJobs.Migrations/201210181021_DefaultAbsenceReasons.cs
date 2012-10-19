using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Raven.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    [Migration("201210181021", "Add default absence reasons configuration")]
    public class DefaultAbsenceReasons : Migration
    {
        private AbsenceReason[] newAbsenceReasons = new[] {
            new AbsenceReason("Enfermedad", "#CB4D4D"), //Red
            new AbsenceReason("Examen", "#DBDB57"), //Yellow
            new AbsenceReason("Maternidad", "#4D77CB"), //Blue
            new AbsenceReason("Paternidad", "#93C"), //Purple
            new AbsenceReason("Fallecimiento de familiar", "#102030"), //DarkGray
            new AbsenceReason("Casamiento", "#A0B0C0"), //LightGray
            new AbsenceReason("Trámites Personales", "#E09952"), //Orange
            new AbsenceReason("Trámites de la empresa", "#34B27D"), //Green
            new AbsenceReason("Cuestiones Personales", "#B27D34"), //Brown
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

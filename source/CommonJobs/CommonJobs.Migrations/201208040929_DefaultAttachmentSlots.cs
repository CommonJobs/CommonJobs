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
    [Migration("201208040929", "Add default attachment slots configuration")]
    public class DefaultAttachmentSlots : Migration
    {
        private AttachmentSlot[] newEmployeeSlots = new[] {
            new AttachmentSlot(typeof(Employee), "Altas")
            {
                Necessity = AttachmentSlotNecessity.MustHave
            },
            new AttachmentSlot(typeof(Employee), "CV")
            {
                Necessity = AttachmentSlotNecessity.MustHave
            },
            new AttachmentSlot(typeof(Employee), "Bajas")
            {
                Necessity = AttachmentSlotNecessity.Optional
            }
        };
        
        public override void Up()
        {
            Down();
            using (var session = DocumentStore.OpenSession())
            {
                var existingSlotIds = new HashSet<string>(LoadExistingSlots(session).Select(x => x.Id));
                var unexistingSlots = newEmployeeSlots.Where(x => !existingSlotIds.Contains(x.Id));

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
                IEnumerable<AttachmentSlot> existingSlots = LoadExistingSlots(session);

                foreach (var slot in existingSlots)
                {
                    session.Delete(slot);
                }

                session.SaveChanges();
            }
        }

        private IEnumerable<AttachmentSlot> LoadExistingSlots(IDocumentSession session)
        {
            IEnumerable<string> ids = newEmployeeSlots.Select(x => x.Id);
            AttachmentSlot[] slots = session.Load<AttachmentSlot>(ids);
            return slots.Where(x => x != null);
        }
    }
}

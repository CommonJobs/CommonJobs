using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Raven.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    public abstract class MoveAttachmentToSlotBaseMigration : Migration
    {
        protected abstract string DownNoteText { get; } 
        protected abstract string SlotId { get; }
        protected abstract string[] UpSearchTexts { get; }

        private void ExtractDataAndDo(RavenJObject result, Action<Employee, string> action)
        {
            var attachmentId = result.Value<string>("AttachmentId");
            var employeeId = result.Value<string>("RelatedEntityId");
            using (var session = DocumentStore.OpenSession())
            {
                var employee = session.Load<Employee>(employeeId);
                action(employee, attachmentId);
                session.SaveChanges();
            }
        }

        private void MoveToSlot(Employee employee, string attachmentId)
        {
            if (employee.AttachmentsBySlot == null)
            {
                employee.AttachmentsBySlot = new List<SlotWithAttachment>();
            }

            var note = employee.Notes.Where(x => x.Attachment != null && x.Attachment.Id == attachmentId).First();
            var slot = employee.AttachmentsBySlot.Where(x => x.SlotId == SlotId).FirstOrDefault();
            if (slot == null)
            {
                slot = new SlotWithAttachment() { SlotId = SlotId };
                employee.AttachmentsBySlot.Add(slot);
            }

            if (slot.Attachment == null)
            {
                slot.Date = note.RegisterDate;
                slot.Attachment = note.Attachment;

                if (string.IsNullOrWhiteSpace(note.Note) || note.Note == DownNoteText)
                {
                    employee.Notes.Remove(note);
                }
                else
                {
                    note.Attachment = null;
                }
            }
        }

        private void MoveFromSlot(Employee employee, string attachmentId)
        {

            var slot = employee.AttachmentsBySlot.Where(x => x.SlotId == SlotId && x.Attachment != null && x.Attachment.Id == attachmentId).First();
            
            var note = new NoteWithAttachment()
            {
                Note = DownNoteText,
                Attachment = slot.Attachment,
                RealDate = slot.Date,
                RegisterDate = slot.Date
            };

            if (employee.Notes == null)
            {
                employee.Notes = new List<NoteWithAttachment>();
            }

            employee.Notes.Add(note);

            employee.AttachmentsBySlot.Remove(slot);
        }

        public override void Up()
        {
            var fullTextFilters = string.Join(" OR ", UpSearchTexts.Select(x => string.Format("FullText:\"{0}\"", x)));

            DoInResults(
                (result) => ExtractDataAndDo(result, MoveToSlot),
                sortedBy: "RelatedEntityId",
                query: string.Format("RelatedEntitySlotId:\"__NOTE__\" AND ({0}) AND RelatedEntityType:\"Employee\"", fullTextFilters),
                index: "Attachments/QuickSearch",
                fieldsToFetch: new[] { "AttachmentId", "RelatedEntityId" });
        }

        public override void Down()
        {
            DoInResults(
                (result) => ExtractDataAndDo(result, MoveFromSlot),
                sortedBy: "RelatedEntityId",
                query: string.Format("RelatedEntitySlotId:\"{0}\" AND RelatedEntityType:\"Employee\"", SlotId),
                index: "Attachments/QuickSearch",
                fieldsToFetch: new[] { "AttachmentId", "RelatedEntityId" });
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    //TODO could this and Employee be refactored into a Person base class? How will be RavenDB affected?
    public class Applicant : Person, IShareableEntity
    {
        public Applicant(): base() { }

        public Applicant(string name) : base(name) { }

        [Display(Name = "Compañías")]
        public List<CompanyHistory> CompanyHistory { get; set; }

        [Display(Name = "Resaltado")]
        public bool IsHighlighted { get; set; }

        [Display(Name = "Notas")]
        public List<NoteWithAttachment> Notes { get; set; }

        [Display(Name = "LinkedIn")]
        public string LinkedInLink { get; set; }

        public void AddNote(NoteWithAttachment note)
        {
            this.Notes.Add(note);
        }
        public void AddGeneralNote(string note, AttachmentReference attachment = null)
        {
            AddNote(new NoteWithAttachment()
            {
                Note = note,
                RealDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Attachment = attachment
            });
        }

        public override IEnumerable<SlotWithAttachment> AllAttachmentReferences
        {
            get { return base.AllAttachmentReferences.Union(SlotWithAttachment.GenerateFromNotes(Notes)); }
        }

        //TODO: automatically remove expired links
        public SharedLinkList SharedLinks { get; set; }

        public string JobSearchId { get; set; }
    }
}

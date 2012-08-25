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
        [Display(Name = "Compañías")]
        public List<CompanyHistory> CompanyHistory { get; set; }

        [Display(Name = "Resaltado")]
        public bool IsHighlighted { get; set; }

        [Display(Name = "Notas")]
        public List<ApplicantNote> Notes { get; set; }

        [Display(Name = "LinkedIn")]
        public string LinkedInLink { get; set; }

        public void AddNote(ApplicantNote note)
        {
            this.Notes.Add(note);
        }
        public void AddGeneralNote(string note, AttachmentReference attachment = null)
        {
            AddNote(new ApplicantNote()
            {
                Note = note,
                NoteType = ApplicantNoteType.GeneralNote,
                RealDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Attachment = attachment
            });
        }

        //TODO replace for "HasInterview"
        public bool HaveInterview
        {
            get { return Notes != null && Notes.Any(x => x.NoteType == ApplicantNoteType.InteviewNote); }
        }

        //TODO replace for "HasTechnicalInterview"
        public bool HaveTechnicalInterview
        {
            get { return Notes != null && Notes.Any(x => x.NoteType == ApplicantNoteType.TechnicalInterviewNote); }
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

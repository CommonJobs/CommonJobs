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

        public override IEnumerable<AttachmentReference> AllAttachmentReferences
        {
            get { return base.AllAttachmentReferences.Union(Notes.EmptyIfNull().Select(x => x.Attachment)).Where(x => x != null); }
        }

        //TODO: automatically remove expired links
        public SharedLinkList SharedLinks { get; set; }

        public string JobSearchId { get; set; }
    }
}

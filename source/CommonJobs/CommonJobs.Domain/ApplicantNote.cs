using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class ApplicantNote : NoteWithAttachment
    {
        public ApplicantNoteType NoteType { get; set; }
    }

    public enum ApplicantNoteType
    {
        GeneralNote = 0,
        InteviewNote = 1,
        TechnicalInterviewNote = 2
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class ApplicantNote : NoteWithAttachment
    {
        [Obsolete]
        public ApplicantNoteType NoteType { get; set; }
    }

    [Obsolete]
    public enum ApplicantNoteType
    {
        GeneralNote = 0,
        InteviewNote = 1,
        TechnicalInterviewNote = 2
    }
}

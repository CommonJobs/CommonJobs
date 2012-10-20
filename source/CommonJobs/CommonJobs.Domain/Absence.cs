using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class Absence : NoteWithAttachment
    {
        public string Reason { get; set; }
        public DateTime? To { get; set; }
        public AbsenceType AbsenceType { get; set; }
        public bool HasCertificate { get; set; }

        //From Event:
        //  public DateTime RealDate { get; set; }
        //  public DateTime RegisterDate { get; set; }
        //  public string Note { get; set; }
    }
}

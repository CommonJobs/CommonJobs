using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class Absence : NoteWithAttachment
    {
        public string Reason { get; set; }
        public DateTime To { get; set; }
        public bool Partial { get; set; }
        public bool HasCertificate { get; set; }
    }
}

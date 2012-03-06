using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class NoteWithAttachment : SimpleNote
    {
        public Attachment Attachment { get; set; }
    }
}

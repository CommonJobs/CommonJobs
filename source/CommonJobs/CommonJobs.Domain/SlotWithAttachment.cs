using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class SlotWithAttachment
    {
        public string SlotId { get; set; }
        public DateTime Date { get; set; }
        public AttachmentReference Attachment { get; set; }
    }
}

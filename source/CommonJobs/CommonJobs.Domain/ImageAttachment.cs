using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class ImageAttachment
    {
        public Attachment Original { get; set; }
        public Attachment Thumbnail { get; set; }
    }
}

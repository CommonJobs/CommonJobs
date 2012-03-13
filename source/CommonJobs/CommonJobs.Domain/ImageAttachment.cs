using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class ImageAttachment
    {
        public AttachmentReference Original { get; set; }
        public AttachmentReference Thumbnail { get; set; }
    }
}

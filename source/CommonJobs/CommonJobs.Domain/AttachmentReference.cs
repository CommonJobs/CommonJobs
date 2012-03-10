using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class AttachmentReference
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        //TODO: Analizar si conviene agregar otros campos como ContentLength or Description?
    }
}

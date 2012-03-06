using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Utilities
{
    public class Attachment
    {
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string ContentType { get; set; }
        //TODO: Analizar si conviene agregar otros campos como ContentLength or Description?
    }
}

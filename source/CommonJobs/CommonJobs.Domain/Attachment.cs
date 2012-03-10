using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class Attachment
    {
        public string Id { get; set; }
        public string ContentType { get; set; }
        public List<string> ExtractorsAvailablesOnIndexation { get; set; }
        public string SuccessfulExtractor { get; set; }
        public string PlainContent { get; set; }
    }
}

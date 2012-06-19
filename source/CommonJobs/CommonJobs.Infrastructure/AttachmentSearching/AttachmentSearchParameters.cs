using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.AttachmentSearching
{
    public class AttachmentSearchParameters : BaseSearchParameters
    {
        public bool SearchOnlyInFileName { get; set; } //TODO: pay attention to this parameter
        public bool OnlyFilesWithText { get; set; } //TODO: pay attention to this parameter
        public OrphansMode Orphans { get; set; }
    }
}

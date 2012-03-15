using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Mvc.UI.Models
{
    public class AttachmentSearchModel
    {
        public enum OrphansMode
        {
            NoOrphans,
            OnlyOrphans,
            All
        }

        public string Term { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public OrphansMode Orphans { get; set; }

        public AttachmentSearchModel()
        {
            Take = 10;
        }
    }
}
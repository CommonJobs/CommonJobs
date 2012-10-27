using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;

namespace CommonJobs.Application.EmployeeSearching
{
    public class EmployeeSearchResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Skills { get; set; }
        public ImageAttachment Photo { get; set; }
        public string CurrentProject { get; set; }
        public string CurrentPosition { get; set; }
        public SlotWithAttachment[] AttachmentsBySlot { get; set; }
    }
}
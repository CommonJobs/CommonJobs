using CommonJobs.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonJobs.Application.ApplicantSearching
{
    public class ApplicantSearchResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string[] Companies { get; set; }
        public string Skills { get; set; }
        public bool IsHighlighted { get; set; }

        public NoteWithAttachment[] Interviews { get; set; }
    }
}
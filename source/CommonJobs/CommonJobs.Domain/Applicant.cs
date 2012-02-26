using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CommonJobs.Domain
{
    //TODO could this and Employee be refactored into a Person base class? How will be RavenDB affected?
    public class Applicant: Person
    {
        [Display(Name = "Compañías")]
        public List<CompanyHistory> CompanyHistory { get; set; }

        //TODO attachments?

        [Display(Name = "Resaltado")]
        public bool IsHighlighted { get; set; }

        [Display(Name = "Notas")]
        public List<ApplicantNote> Notes { get; set; }
    }
}

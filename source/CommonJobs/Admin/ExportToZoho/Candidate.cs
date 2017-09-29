using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Admin.ExportToZoho
{
    public class Candidate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string SkypeId { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int? ExperienceInYears { get; set; }
        public decimal? CurrentSalary { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string CurrentEmployer { get; set; }
        public string AdditionalInfo { get; set; }
        public string Source { get; set; } = "CommonJobs";
        public string LinkedIn { get; set; }
        public string CandidateStatus { get; set; }
        public string Perfiles { get; set; } // Separado por ;?
        public string StackPredominante { get; set; } // Separado por ;
        public string PersonalWebsite { get; set; }
        public bool? IsHotCandidate { get; set; }
    }
}

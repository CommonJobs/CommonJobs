using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RavenPOC1.Domain
{
    public class Applicant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<string> Phones { get; set; }
        public List<string> Skills { get; set; }
        public DateTime BirthDate { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public List<string> AdvertisementIds { get; set; }
    }
}

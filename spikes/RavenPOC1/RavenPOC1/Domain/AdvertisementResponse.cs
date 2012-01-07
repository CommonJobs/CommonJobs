using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RavenPOC1.Domain
{
    public class AdvertisementResponse
    {
        public int Id { get; set; }
        public int AdvertisementId { get; set; }
        public int ApplicantId { get; set; }
    }
}

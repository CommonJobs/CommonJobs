using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class SharedLink
    {
        public string Url { get; set; }
        public string FriendlyName { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

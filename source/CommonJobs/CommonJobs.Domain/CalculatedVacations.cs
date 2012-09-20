using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class CalculatedVacations
    {
        public int TotalTaken { get; set; }
        public int TotalPending { get; set; }
        public Dictionary<int, CalculatedYearVacations> ByYear { get; set; }
    }

    public class CalculatedYearVacations
    {
        public int Antiquity { get; set; }
        public int Earned { get; set; }
        public int Taken { get; set; }
        public int Pending { get; set; }
    }

}

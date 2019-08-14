using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.Evaluations
{
    public class CalificationItem
    {
        public string Key { get; set; }

        public decimal? Value { get; set; }

        public string Comment { get; set; }

        public string GroupKey { get; set; }
    }
}

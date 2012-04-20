using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure
{
    public class BaseSearchParameters
    {
        public string Term { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}

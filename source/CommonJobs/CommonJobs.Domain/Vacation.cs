using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public struct Vacation
    {
        public int Period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}

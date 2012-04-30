using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class VacationList: List<Vacation>
    {
        public int TotalDays
        {
            get
            {
                return this.Sum(x => x.TotalDays);
            }
        }
    }
}

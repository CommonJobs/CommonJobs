using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public struct MenuItem : IWeekDayOptionKeyed
    {
        public int WeekIdx { get; set; }
        public int DayIdx { get; set; }
        public string OptionKey { get; set; }
        public string Food { get; set; }
    }
}

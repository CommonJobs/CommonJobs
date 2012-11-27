using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public struct DailyMenuRequestSummaryItem
    {
        public string PlaceKey { get; set; }
        public string OptionKey { get; set; }
        public int Quantity { get; set; }
    }
}

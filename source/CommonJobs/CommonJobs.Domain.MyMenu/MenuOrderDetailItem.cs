using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public struct MenuOrderDetailItem
    {
        public string PlaceKey { get; set; }
        public string OptionKey { get; set; }
        public string EmployeeName { get; set; }
        public string Comment { get; set; }
    }
}

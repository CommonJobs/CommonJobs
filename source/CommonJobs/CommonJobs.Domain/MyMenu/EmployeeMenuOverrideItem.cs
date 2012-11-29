using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public struct EmployeeMenuOverrideItem
    {
        public DateTime Date { get; set; }
        public bool Cancel { get; set; }
        public string OptionKey { get; set; }
        public string PlaceKey { get; set; }
        public string Comment { get; set; }
    }
}

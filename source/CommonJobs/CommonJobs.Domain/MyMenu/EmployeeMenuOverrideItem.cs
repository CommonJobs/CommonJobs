using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class EmployeeMenuOverrideItem
    {
        public DateTime date { get; set; }
        public bool cancel { get; set; }
        public string option { get; set; }
        public string place { get; set; }
        public string comment { get; set; }
    }
}

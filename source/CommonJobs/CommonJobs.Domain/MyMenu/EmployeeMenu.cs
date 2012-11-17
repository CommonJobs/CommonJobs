using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class EmployeeMenu
    {
        public string Id { get; set; }
        public string userName { get; set; }
        public string menuId { get; set; }
        public string name { get; set; }
        public string defaultPlace { get; set; }
        public List<EmployeeMenuItem> choices { get; set; }
        public List<EmployeeMenuOverrideItem> overrides { get; set; }
    }
}

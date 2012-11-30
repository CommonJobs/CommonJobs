using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class EmployeeMenu
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string MenuId { get; set; }
        public string EmployeeName { get; set; }
        public string DefaultPlaceKey { get; set; }
        public WeekDayKeyedCollection<EmployeeMenuItem> WeeklyChoices { get; set; }
        public List<EmployeeMenuOverrideItem> Overrides { get; set; }

        public EmployeeMenu()
        {
            DefaultPlaceKey = "";
            WeeklyChoices = new WeekDayKeyedCollection<EmployeeMenuItem>();
            Overrides = new List<EmployeeMenuOverrideItem>();
        }
    }
}

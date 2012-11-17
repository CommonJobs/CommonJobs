using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class Menu
    {
        public Menu()
        {
            options = new List<Option>();
            places = new List<Place>();
            foods = new List<MenuItem>();
        }

        public string title { get; set; }
        public int firstWeek { get; set; }
        public int weeksQuantity { get; set; }
        public List<Option> options { get; set; }
        public List<Place> places { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string deadlineTime { get; set; }
        public List<MenuItem> foods { get; set; }
    }
}

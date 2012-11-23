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

        public string Id { get; set; }
        public string title { get; set; }
        public int firstWeek { get; set; }
        public int weeksQuantity { get; set; }
        public List<Option> options { get; set; }
        public List<Place> places { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime lastSentDate { get; set; }
        public string deadlineTime { get; set; }
        public List<MenuItem> foods { get; set; }

        public DateTime CalculateNextExecutionTime(DateTime now)
        {
            if (endDate < now)
                return DateTime.MaxValue;

            //TODO: It will fail if the deadlineTime is near to 00:00
            var date = startDate > now.Date ? startDate : now.Date;
            if (lastSentDate.Date >= date)
                date = date.AddDays(1);

            var deadlineTS = TimeSpan.Zero;
            TimeSpan.TryParse(deadlineTime, out deadlineTS);
            return date.Add(deadlineTS);
        }

        public string GetTaskId()
        {
            return string.Format("{0}/Task", Id);
        }
    }
}

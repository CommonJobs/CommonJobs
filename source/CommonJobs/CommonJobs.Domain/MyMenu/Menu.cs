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
            Options = new List<Option>();
            Places = new List<Place>();
            Foods = new List<MenuItem>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public int FirstWeekIdx { get; set; }
        public int WeeksQuantity { get; set; }
        public List<Option> Options { get; set; }
        public List<Place> Places { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LastSentDate { get; set; }
        public string DeadlineTime { get; set; }
        public List<MenuItem> Foods { get; set; }

        public DateTime CalculateNextExecutionTime(DateTime now)
        {
            if (EndDate < now)
                return DateTime.MaxValue;

            //TODO: It will fail if the deadlineTime is near to 00:00
            var date = StartDate > now.Date ? StartDate : now.Date;
            if (LastSentDate.Date >= date)
                date = date.AddDays(1);

            var deadlineTS = TimeSpan.Zero;
            TimeSpan.TryParse(DeadlineTime, out deadlineTS);
            return date.Add(deadlineTS);
        }

        public string GetTaskId()
        {
            return string.Format("{0}/Task", Id);
        }
    }
}

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
            Options = new StringKeyedCollection<Option>();
            Places = new StringKeyedCollection<Place>();
            Foods = new WeekDayOptionKeyedCollection<MenuItem>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public int FirstWeekIdx { get; set; }
        public int WeeksQuantity { get; set; }
        public StringKeyedCollection<Option> Options { get; set; }
        public StringKeyedCollection<Place> Places { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime LastOrderDate { get; set; }
        public string DeadlineTime { get; set; }
        public WeekDayOptionKeyedCollection<MenuItem> Foods { get; set; }

        public DateTime CalculateNextExecutionTime(DateTime now)
        {
            if (EndDate < now)
                return DateTime.MaxValue;

            //TODO: It will fail if the deadlineTime is near to 00:00
            var date = StartDate > now.Date ? StartDate : now.Date;
            if (LastOrderDate.Date >= date)
                date = date.AddDays(1);

            var deadlineTS = TimeSpan.Zero;
            TimeSpan.TryParse(DeadlineTime, out deadlineTS);
            return date.Add(deadlineTS);
        }

        public string GetTaskId()
        {
            return string.Format("{0}/Task", Id);
        }

        public WeekDayKey GetWeekDay(DateTime date)
        {
            return new WeekDayKey
            {
                DayIdx = (int)date.DayOfWeek,
                WeekIdx = GetWeek(date)
            };
        }

        public int GetWeek(DateTime date)
        {
            var startDateDayIdx = (int)StartDate.DayOfWeek;
            var zeroDay = StartDate.Date
                .AddDays(-1 * startDateDayIdx) //Domingo
                .AddDays(-7 * FirstWeekIdx); //Semana Cero

            var dayIdx = (int)date.DayOfWeek;
            var sundayDate = date.AddDays(-1 * dayIdx);
            var diff = (sundayDate - zeroDay).Days / 7;
            var result = diff < 0 ?
                WeeksQuantity + diff % WeeksQuantity
                : diff % WeeksQuantity;
            return result;
        }

    }
}

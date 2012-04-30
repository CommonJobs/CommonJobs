using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class Vacation
    {
        public int Period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int TotalDays
        {
            get
            {
                var totalDays = 0;
                for (var date = From; date < To; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday
                        && date.DayOfWeek != DayOfWeek.Sunday)
                        totalDays++;
                }

                return totalDays;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class WeekDayKeyedCollection<T> : KeyedCollection<WeekDayKey, T>
        where T : IWeekDayKeyed
    {
        protected override WeekDayKey GetKeyForItem(T item)
        {
            return new WeekDayKey() { DayIdx = item.DayIdx, WeekIdx = item.WeekIdx };
        }
    }

    public struct WeekDayKey
    {
        public int WeekIdx { get; set; }
        public int DayIdx { get; set; }
    }

    public interface IWeekDayKeyed
    {
        int WeekIdx { get; }
        int DayIdx { get; }
    }

}

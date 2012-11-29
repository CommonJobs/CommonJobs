using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class WeekDayOptionKeyedCollection<T> : KeyedCollection<WeekDayOptionKey, T>
        where T :  IWeekDayOptionKeyed
    {
        protected override WeekDayOptionKey GetKeyForItem(T item)
        {
            return new WeekDayOptionKey() { DayIdx = item.DayIdx, WeekIdx = item.WeekIdx, OptionKey = item.OptionKey };
        }
    }

    public struct WeekDayOptionKey
    {
        public int WeekIdx { get; set; }
        public int DayIdx { get; set; }
        public string OptionKey { get; set; }
    }

    public interface IWeekDayOptionKeyed
    {
        int WeekIdx { get; }
        int DayIdx { get; }
        string OptionKey { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public class DailyMenuRequest
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string MenuId { get; set; }
        public int WeekIdx { get; set; }
        public int DayIdx { get; set; }
        public StringKeyedCollection<Place> Places { get; set; }
        public StringKeyedCollection<Option> Options { get; set; }
        public Dictionary<string, string> FoodsByOption { get; set; }
        public List<DailyMenuRequestSummaryItem> Summary { get; set; }
        public List<DailyMenuRequestDetailItem> Detail { get; set; }

        private DailyMenuRequest()
        {
            //RavenDB use
            Detail = new List<DailyMenuRequestDetailItem>();
            Summary = new List<DailyMenuRequestSummaryItem>();
            Places = new StringKeyedCollection<Place>();
            Options = new StringKeyedCollection<Option>();
            FoodsByOption = new Dictionary<string, string>();
        }

        public DailyMenuRequest(Menu menu, DateTime date, IEnumerable<EmployeeMenu> employeeMenues)
        {
            Date = date.Date;
            Id = string.Format("{0}/{1:yyyy-MM-dd}", menu.Id, Date);
            MenuId = menu.Id;

            Detail = new List<DailyMenuRequestDetailItem>();
            Summary = new List<DailyMenuRequestSummaryItem>();
            Places = menu.Places;
            Options = menu.Options;

            var dayWeek = menu.GetWeekDay(Date);
            DayIdx = dayWeek.DayIdx;
            WeekIdx = dayWeek.WeekIdx;

            FoodsByOption = Options
                .Select(x => menu.Foods.GetItemSecurely(new WeekDayOptionKey() { WeekIdx = WeekIdx, DayIdx = DayIdx, OptionKey = x.Key }))
                .Where(x => !string.IsNullOrWhiteSpace(x.Food))
                .ToDictionary(x => x.OptionKey, x => x.Food);

            employeeMenues = employeeMenues ?? Enumerable.Empty<EmployeeMenu>();

            foreach (var employeeMenu in employeeMenues)
            {
                Detail.Add(CreateDetailItem(employeeMenu));
            }

            Summary = Detail
                .Where(x => IsNotEmpty(x.PlaceKey) && IsNotEmpty(x.OptionKey))
                .GroupBy(x => new { x.OptionKey, x.PlaceKey })
                .Select(x => new DailyMenuRequestSummaryItem() { OptionKey = x.Key.OptionKey, PlaceKey = x.Key.PlaceKey, Quantity = x.Count() })
                .ToList();
        }

        #region private helper methods
        private bool IsNotEmpty(string key)
        {
            return !string.IsNullOrWhiteSpace(key);
        }

        private bool IsValidPlace(string placeKey)
        {
            return Places.Contains(placeKey);
        }

        private bool IsNotEmptyValidPlace(string placeKey)
        {
            return IsNotEmpty(placeKey) && IsValidPlace(placeKey);
        }

        private string GetPlaceOrDefault(string placeKey, string defaultValue)
        {
            return IsNotEmptyValidPlace(placeKey) ? placeKey : defaultValue;
        }

        private bool IsValidOption(string optionKey)
        {
            return Options.Contains(optionKey) && FoodsByOption.ContainsKey(optionKey) && !string.IsNullOrWhiteSpace(FoodsByOption[optionKey]);
        }

        private bool IsNotEmptyValidOption(string optionKey)
        {
            return IsNotEmpty(optionKey) && IsValidOption(optionKey);
        }

        private string GetOptionOrDefault(string optionKey, string defaultValue)
        {
            return IsNotEmptyValidOption(optionKey) ? optionKey : defaultValue;
        }


        #endregion

        private DailyMenuRequestDetailItem CreateDetailItem(EmployeeMenu employeeMenu)
        {
            string placeKey = null;
            string optionKey = null;

            placeKey = GetPlaceOrDefault(employeeMenu.DefaultPlaceKey, placeKey);

            var dayChoices = employeeMenu.WeeklyChoices.GetItemSecurely(new WeekDayKey() { WeekIdx = WeekIdx, DayIdx = DayIdx });

            placeKey = GetPlaceOrDefault(dayChoices.PlaceKey, placeKey);
            optionKey = GetOptionOrDefault(dayChoices.OptionKey, optionKey);

            var lastOverride = employeeMenu.Overrides.Where(x => x.Date.Date == Date).LastOrDefault();

            if (!lastOverride.Cancel)
            {
                placeKey = GetPlaceOrDefault(lastOverride.PlaceKey, placeKey);
                optionKey = GetOptionOrDefault(lastOverride.OptionKey, optionKey);
            }

            if (lastOverride.Cancel || string.IsNullOrWhiteSpace(placeKey) || string.IsNullOrWhiteSpace(optionKey))
            {
                placeKey = null;
                optionKey = null;
            }

            var comment = IsNotEmpty(lastOverride.Comment) ? lastOverride.Comment : null;

            return new DailyMenuRequestDetailItem()
            {
                EmployeeUserName = employeeMenu.UserName,
                EmployeeName = employeeMenu.EmployeeName,
                PlaceKey = placeKey,
                OptionKey = optionKey,
                Comment = comment
            };
        }
    }
}

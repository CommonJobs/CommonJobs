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

        public DailyMenuRequest(Menu menu, DateTime date)
            : this()
        {
            Date = date.Date;
            Id = string.Format("{0}/{1:yyyy-MM-dd}", menu.Id, Date);
            foreach (var place in menu.Places)
            {
                Places.Add(place);
            }
            foreach (var option in menu.Options)
            {
                Options.Add(option);
                //menu.GetFood(Date, option.Key)
            }

        }

        public void AddEmployeeMenu(EmployeeMenu employeeMenu) 
        {

        }        
        
    }
}

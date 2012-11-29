using CommonJobs.Domain;
using CommonJobs.Domain.MyMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class EmployeeMenuDTO
    {
        public EmployeeMenu EmployeeMenu { get; set; }
        public Menu MenuDefinition { get; set; }
        public LastRequestEmployeeMenuDTO LastRequest { get; set; }
        

        public static EmployeeMenuDTO Create(Employee employee, Menu menuDefinition, EmployeeMenu employeeMenu, DailyMenuRequest lastRequest)
        {
            employeeMenu.EmployeeName = string.Format("{0}, {1}", employee.LastName, employee.FirstName);
            
            var result = new EmployeeMenuDTO
            {
                EmployeeMenu = employeeMenu,
                MenuDefinition = menuDefinition
            };

            if (lastRequest != null && lastRequest.DetailByUserName.ContainsKey(employee.UserName))
            {
                var detail = lastRequest.DetailByUserName[employee.UserName];
                result.LastRequest = new LastRequestEmployeeMenuDTO()
                {
                    Date = menuDefinition.LastSentDate,
                    Option = lastRequest.OptionsByKey[detail.OptionKey],
                    Place = lastRequest.PlacesByKey[detail.PlaceKey],
                    Comment = detail.Comment,
                    Food = lastRequest.FoodsByOption[detail.OptionKey],
                    WeekIdx = lastRequest.WeekIdx,
                    DayIdx = lastRequest.DayIdx
                };
            }

            return result;
        }
    }

    public class LastRequestEmployeeMenuDTO
    {
        public DateTime Date { get; set; }
        public int WeekIdx { get; set; }
        public int DayIdx { get; set; }
        public string Option { get; set; }
        public string Place { get; set; }
        public string Comment { get; set; }
        public string Food { get; set; }
    }
}

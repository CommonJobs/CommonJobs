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
                    OptionKey = detail.OptionKey,
                    PlaceKey = detail.PlaceKey,
                    Comment = detail.Comment,
                    Food = lastRequest.FoodsByOption[detail.OptionKey],
                };
            }

            return result;
        }
    }

    public class LastRequestEmployeeMenuDTO
    {
        public DateTime Date { get; set; }
        public string OptionKey { get; set; }
        public string PlaceKey { get; set; }
        public string Comment { get; set; }
        public string Food { get; set; }
    }
}

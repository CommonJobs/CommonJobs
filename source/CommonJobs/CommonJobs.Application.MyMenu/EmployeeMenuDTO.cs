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
        public EmployeeMenuOrderDTO LastOrder { get; set; }


        public static EmployeeMenuDTO Create(Employee employee, Menu menuDefinition, EmployeeMenu employeeMenu, MenuOrder todayOrder, bool hasTomorrowBeenOrdered = false)
        {
            employeeMenu.EmployeeName = string.Format("{0}, {1}", employee.LastName, employee.FirstName);

            var result = new EmployeeMenuDTO
            {
                EmployeeMenu = employeeMenu,
                MenuDefinition = menuDefinition
            };

            if (todayOrder != null && todayOrder.DetailByUserName.ContainsKey(employee.UserName))
            {
                var detail = todayOrder.DetailByUserName[employee.UserName];
                result.LastOrder = new EmployeeMenuOrderDTO()
                {
                    Date = todayOrder.Date,
                    Option = detail.OptionKey == null ? null : todayOrder.OptionsByKey[detail.OptionKey],
                    Place = detail.PlaceKey == null ? null : todayOrder.PlacesByKey[detail.PlaceKey],
                    Comment = detail.Comment,
                    Food = detail.OptionKey == null ? null : todayOrder.FoodsByOption[detail.OptionKey],
                    WeekIdx = todayOrder.WeekIdx,
                    DayIdx = todayOrder.DayIdx,
                    IsOrdered = todayOrder.IsOrdered,
                    HasTomorrowBeenOrdered = hasTomorrowBeenOrdered
                };
            }
            else if (todayOrder != null)
            {
                result.LastOrder = new EmployeeMenuOrderDTO()
                {
                    Date = todayOrder.Date,
                    Option = null,
                    Place = null,
                    Comment = "// El usuario no existía al momento de hacer el pedido //",
                    Food = null,
                    WeekIdx = todayOrder.WeekIdx,
                    DayIdx = todayOrder.DayIdx,
                    IsOrdered = todayOrder.IsOrdered,
                    HasTomorrowBeenOrdered = hasTomorrowBeenOrdered
                };
            }

            return result;
        }
    }

    public class EmployeeMenuOrderDTO
    {
        public DateTime Date { get; set; }
        public int WeekIdx { get; set; }
        public int DayIdx { get; set; }
        public string Option { get; set; }
        public string Place { get; set; }
        public string Comment { get; set; }
        public string Food { get; set; }
        public bool IsOrdered { get; set; }
        public bool HasTomorrowBeenOrdered { get; set; }
    }
}

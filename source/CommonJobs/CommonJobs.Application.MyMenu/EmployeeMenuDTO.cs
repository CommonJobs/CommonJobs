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
        

        public static EmployeeMenuDTO Create(Employee employee, Menu menuDefinition, EmployeeMenu employeeMenu, MenuOrder lastOrder)
        {
            employeeMenu.EmployeeName = string.Format("{0}, {1}", employee.LastName, employee.FirstName);
            
            var result = new EmployeeMenuDTO
            {
                EmployeeMenu = employeeMenu,
                MenuDefinition = menuDefinition
            };

            if (lastOrder != null && lastOrder.DetailByUserName.ContainsKey(employee.UserName))
            {
                var detail = lastOrder.DetailByUserName[employee.UserName];
                result.LastOrder = new EmployeeMenuOrderDTO()
                {
                    Date = menuDefinition.LastOrderDate,
                    Option = detail.OptionKey == null ? null : lastOrder.OptionsByKey[detail.OptionKey],
                    Place = detail.PlaceKey == null ? null : lastOrder.PlacesByKey[detail.PlaceKey],
                    Comment = detail.Comment,
                    Food = detail.OptionKey == null ? null : lastOrder.FoodsByOption[detail.OptionKey],
                    WeekIdx = lastOrder.WeekIdx,
                    DayIdx = lastOrder.DayIdx,
                    IsOrdered = true
                };
            }
            else if (lastOrder != null)
            {
                result.LastOrder = new EmployeeMenuOrderDTO()
                {
                    Date = menuDefinition.LastOrderDate,
                    Option = null,
                    Place = null,
                    Comment = "// El usuario no existía al momento de hacer el pedido //",
                    Food = null,
                    WeekIdx = lastOrder.WeekIdx,
                    DayIdx = lastOrder.DayIdx,
                    IsOrdered = true
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
    }
}

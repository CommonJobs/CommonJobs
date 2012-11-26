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

        public static EmployeeMenuDTO Create(Employee employee, Menu menuDefinition, EmployeeMenu employeeMenu)
        {
            employeeMenu.EmployeeName = string.Format("{0}, {1}", employee.LastName, employee.FirstName);
            return new EmployeeMenuDTO
            {
                EmployeeMenu = employeeMenu,
                MenuDefinition = menuDefinition
            };
        }
    }
}

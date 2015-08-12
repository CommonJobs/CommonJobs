using CommonJobs.Application.EmployeeSearching;
using CommonJobs.Domain;
using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
using NLog;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonJobs.Application.MyMenu
{
    public class GetEmployeeMenuCommand : Command<EmployeeMenuDTO>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public String UserName { get; set; }

        public GetEmployeeMenuCommand(string userName)
        {
            this.UserName = userName;
        }

        public override EmployeeMenuDTO ExecuteWithResult()
        {
            RavenQueryStatistics stats;

            //TODO: tener en cuenta TerminationDate y si come o no
            var employee = RavenSession
                .Query<Employee, EmployeeByUserName_Search>()
                .Statistics(out stats)
                .Where(x => x.UserName == UserName)
                .FirstOrDefault();

            if (stats.TotalResults > 1)
                throw new ApplicationException(string.Format("Error: Hay más de un empleado para el username {0}.", UserName));

            if (employee == null)
                throw new ApplicationException(string.Format("El empleado con el username {0} no existe en la base de datos de CommonJobs", UserName));

            var employeeMenu = RavenSession.Include<EmployeeMenu>(x => x.MenuId).Load<EmployeeMenu>(Common.GenerateEmployeeMenuId(UserName));
            if (employeeMenu == null)
            {
                employeeMenu = CreateDefaultEmployeeMenu(UserName, Common.DefaultMenuId);
                RavenSession.Store(employeeMenu);
            }

            var menuDefinition = ExecuteCommand(new GetMenuDefinitionCommand(employeeMenu.MenuId));

            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);
            var todayOrderId = MenuOrder.GenerateId(employeeMenu.MenuId, today);

            var todayOrder = RavenSession.Load<MenuOrder>(todayOrderId);

            // IS THIS NECESSARY? The orders are created with IsOrdered = true
            //if (todayOrder != null)
            //{
            //    todayOrder.IsOrdered = true;
            //}

            // If the last order generated is from tomorrow, then we should tell the user
            var hasTomorrowBeenOrdered = (menuDefinition.LastGeneratedOrderDate.Date == tomorrow);

            return EmployeeMenuDTO.Create(employee, menuDefinition, employeeMenu, todayOrder, hasTomorrowBeenOrdered);
        }

        private static EmployeeMenu CreateDefaultEmployeeMenu(string username, string menuId)
        {
            return new EmployeeMenu()
            {
                Id = Common.GenerateEmployeeMenuId(username),
                MenuId = menuId,
                UserName = username,
                EmployeeName = username,
                DefaultPlaceKey = "",
                WeeklyChoices = new WeekDayKeyedCollection<EmployeeMenuItem>(),
                Overrides = new List<EmployeeMenuOverrideItem>()
            };
        }
    }
}

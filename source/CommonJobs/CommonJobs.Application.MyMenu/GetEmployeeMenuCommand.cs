using CommonJobs.Domain;
using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
using NLog;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                .Query<Employee>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
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
            
            var lastOrder = RavenSession.Load<MenuOrder>(MenuOrder.GenerateId(employeeMenu.MenuId, menuDefinition.LastOrderDate));
            if (lastOrder != null)
            {
                lastOrder.IsOrdered = true;
            }
            
            return EmployeeMenuDTO.Create(employee, menuDefinition, employeeMenu, lastOrder);
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

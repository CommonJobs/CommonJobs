using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb.Schedule;
using Newtonsoft.Json;
using NLog;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class ProcessMenuCommand : SchedulableCommand
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public string MenuDefinitionId { get; set; }
        
        public override void Execute()
        {
            var menuDefinition = ExecuteCommand(new GetMenuDefinitionCommand(MenuDefinitionId));
            var employeeMenus = ExecuteCommand(new GetEmployeeMenusCommand() { MenuDefinitionId = menuDefinition.Id });
            var order = new MenuOrder(menuDefinition, Now(), employeeMenus);
            order.IsOrdered = true;
            RavenSession.Store(order);
            menuDefinition.LastOrderDate = Now();
        }

        protected override DateTime CalculateNextExecutionTime(DateTime start, DateTime scheduled)
        {
            var menuDefinition = ExecuteCommand(new GetMenuDefinitionCommand(MenuDefinitionId));
            return menuDefinition.CalculateNextExecutionTime(start);
        }

        protected override bool IsExecutionRequired()
        {
            var menuDefinition = ExecuteCommand(new GetMenuDefinitionCommand(MenuDefinitionId));
            return CalculateNextExecutionTime(menuDefinition.LastOrderDate, menuDefinition.LastOrderDate) <= Now();
        }
    }
}

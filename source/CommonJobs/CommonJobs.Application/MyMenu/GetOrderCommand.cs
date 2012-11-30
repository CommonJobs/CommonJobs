using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
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
    public class GetOrderCommand : Command<MenuOrder>
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public string MenuDefinitionId { get; set; }
        public DateTime Date { get; set; }

        private MenuOrder GetExistingOrder()
        {
            var order = RavenSession.Load<MenuOrder>(MenuOrder.GenerateId(string.IsNullOrWhiteSpace(MenuDefinitionId) ? Common.DefaultMenuId : MenuDefinitionId, Date));
            if (order != null)
            {
                order.IsOrdered = true;
            }
            return order;
        }

        private MenuOrder GeneratePreviewOrder()
        {
            var menuDefinition = ExecuteCommand(new GetMenuDefinitionCommand(MenuDefinitionId));
            var employeeMenus = ExecuteCommand(new GetEmployeeMenusCommand() { MenuDefinitionId = menuDefinition.Id });
            var order = new MenuOrder(menuDefinition, Date, employeeMenus);
            order.IsOrdered = false;
            return order;
        }

        public override MenuOrder ExecuteWithResult()
        {
            var order = GetExistingOrder();
            if (order == null)
                order = GeneratePreviewOrder();
            return order;
        }
    }
}

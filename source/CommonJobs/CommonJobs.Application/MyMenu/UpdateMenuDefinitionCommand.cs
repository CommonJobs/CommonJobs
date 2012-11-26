using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
using CommonJobs.Infrastructure.RavenDb.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class UpdateMenuDefinitionCommand : Command
    {
        public Menu MenuDefinition { get; private set; }
        public DateTime Now { get; private set; }

        public UpdateMenuDefinitionCommand(Menu menuDefinition, DateTime now)
        {
            MenuDefinition = menuDefinition;
            if (String.IsNullOrWhiteSpace(menuDefinition.Id))
            {
                menuDefinition.Id = Common.DefaultMenuId;
            }
            Now = now;
        }

        public override void Execute()
        {
            RavenSession.Store(MenuDefinition);

            var nextExecution = MenuDefinition.CalculateNextExecutionTime(Now);
            var task = new ScheduleTask() { 
                Id = MenuDefinition.GetTaskId(), 
                IsActive = nextExecution < DateTime.MaxValue, 
                Command = new ProcessMenuCommand() { MenuDefinitionId = MenuDefinition.Id },
                NextExecution = nextExecution
            };
            RavenSession.Store(task);
        }
    }
}

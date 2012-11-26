using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb.Schedule;
using Newtonsoft.Json;
using NLog;
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
            var menuDefinition = RavenSession.Load<Menu>(MenuDefinitionId);
            //TODO: send
            menuDefinition.LastSentDate = Now();
        }

        protected override DateTime CalculateNextExecutionTime(DateTime start, DateTime scheduled)
        {
            var menuDefinition = RavenSession.Load<Menu>(MenuDefinitionId);
            return menuDefinition.CalculateNextExecutionTime(start);
        }
    }
}

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

        private IEnumerable<EmployeeMenu> GetEmployeeMenus()
        {
            RavenQueryStatistics stats = null;
            List<EmployeeMenu> result = new List<EmployeeMenu>();
            var skip = 0;
            var page = 255;

            while (stats == null || skip < stats.TotalResults)
            {
                var qry = RavenSession
                    .Query<EmployeeMenu>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                    .Statistics(out stats)
                    .Where(x => x.MenuId == MenuDefinitionId)
                    .Skip(skip)
                    .Take(page);

                result.AddRange(qry);
                skip += page;
            }
            return result;
        }
        
        public override void Execute()
        {
            var menuDefinition = RavenSession.Load<Menu>(MenuDefinitionId);
            var employeeMenus = GetEmployeeMenus();
            var request = new DailyMenuRequest(menuDefinition, Now(), employeeMenus);
            RavenSession.Store(request);
            menuDefinition.LastSentDate = Now();
        }

        protected override DateTime CalculateNextExecutionTime(DateTime start, DateTime scheduled)
        {
            var menuDefinition = RavenSession.Load<Menu>(MenuDefinitionId);
            return menuDefinition.CalculateNextExecutionTime(start);
        }
    }
}

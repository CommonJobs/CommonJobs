using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
using CommonJobs.Infrastructure.RavenDb.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class UpdateEmployeeMenuCommand : Command
    {
        public EmployeeMenu EmployeeMenu { get; private set; }

        public UpdateEmployeeMenuCommand(EmployeeMenu employeeMenu)
        {
            EmployeeMenu = employeeMenu;
        }

        public override void Execute()
        {
            EmployeeMenu.Id = Common.GenerateEmployeeMenuId(EmployeeMenu.UserName);
            RavenSession.Store(EmployeeMenu);
        }
    }
}

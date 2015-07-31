using CommonJobs.Domain.MyMenu;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class EmployeeMenuByMenuId_Search : AbstractIndexCreationTask<EmployeeMenu>
    {
        public EmployeeMenuByMenuId_Search()
        {
            Map = m => m.Select(e => new { e.MenuId });
        }
    }
}

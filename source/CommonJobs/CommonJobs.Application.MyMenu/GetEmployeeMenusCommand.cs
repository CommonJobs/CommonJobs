﻿using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.RavenDb;
using Raven.Client;
using Raven.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    public class GetEmployeeMenusCommand : Command<IEnumerable<EmployeeMenu>>
    {
        public string MenuDefinitionId { get; set; }

        public override IEnumerable<EmployeeMenu> ExecuteWithResult()
        {
            RavenQueryStatistics stats = null;
            List<EmployeeMenu> result = new List<EmployeeMenu>();
            var skip = 0;
            var page = 255;

            while (stats == null || skip < stats.TotalResults)
            {
                var qry = RavenSession
                    .Query<EmployeeMenu, EmployeeMenuByMenuId_Search>()
                    .Statistics(out stats)
                    .Where(x => x.MenuId == MenuDefinitionId)
                    .Skip(skip)
                    .Take(page);

                result.AddRange(qry);
                skip += page;
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Linq;
using Raven.Json.Linq;
using CommonJobs.Domain.MyMenu;

namespace CommonJobs.Migrations
{
    [Migration("201212010811", "Create default menu entries for some users imported using default places from Import201212010811.json file")]
    public class CreateDefaultMenus : Migration
    {
        public override void Up()
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath)), "Import201212010811.json");
            var serializer = new Newtonsoft.Json.JsonSerializer();
            var placeByUserName = serializer.Deserialize <Dictionary<string, string>>(new StreamReader(filePath));
            using (var session = DocumentStore.OpenSession())
            {
                var empty = false;
                var skip = 0;
                var take = 20;
                while (!empty) {
                    var result = session.Query<Employee>()
                        .Where(x => x.UserName != null && x.UserName != "")
                        .OrderBy(x => x.UserName)
                        .Skip(skip)
                        .Take(take)
                        .ToArray();

                    foreach (var employee in result)
                    {
                        string placeKey = null;
                        bool createMenu = false;
                        if (!string.IsNullOrWhiteSpace(employee.UserName) && placeByUserName.TryGetValue(employee.UserName, out placeKey) && placeKey != "-")
                        {
                            createMenu = true;
                        }
                        else if (!string.IsNullOrWhiteSpace(employee.UserName) && !placeByUserName.ContainsKey(employee.UserName))
                        {
                            if (!string.IsNullOrWhiteSpace(employee.Platform) && (employee.Platform.ToUpper().Contains("MDQ") || employee.Platform.ToUpper().Contains("MDP")  || employee.Platform.ToUpper().Contains("MAR")))
                            {
                                createMenu = true;
                                placeKey = employee.Platform.ToUpper().Contains("GARAY") ? "place_garay"
                                    : employee.Platform.ToUpper().Contains("RIOJA") ? "place_larioja"
                                    : null;
                            }
                        }

                        if (createMenu) 
                        {
                            var employeeMenu = new EmployeeMenu()
                            {
                                Id = "Menu/" + employee.UserName,
                                MenuId = "Menu/DefaultMenu",
                                UserName = employee.UserName,
                                EmployeeName = string.Format("{0}, {1}", employee.LastName, employee.FirstName),
                                DefaultPlaceKey = placeKey,
                                WeeklyChoices = new WeekDayKeyedCollection<EmployeeMenuItem>(),
                                Overrides = new List<EmployeeMenuOverrideItem>()
                            };
                            session.Store(employeeMenu);
                        }
                    }

                    empty = result.Length == 0;
                    skip += take;
                }
                session.SaveChanges();
            }
        }

        public override void Down()
        {
        }

    }
}

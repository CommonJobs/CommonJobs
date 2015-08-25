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

namespace CommonJobs.Migrations
{
    [Migration("201210171646", "Update employees email and AD users from Import201210171646.json file")]
    public class UpdateEmployeesEmailUser : Migration
    {
        private struct ImportKey
        {
            public readonly string FirstName;
            public readonly string LastName;

            private ImportKey(string lastName, string firstName)
            {
                FirstName = (firstName ?? string.Empty).Trim();
                LastName = (lastName ?? string.Empty).Trim();
            }

            public ImportKey(ImportItem importItem)
                : this(importItem.CurrentLastName, importItem.CurrentFirstName)
            {
            }

            public ImportKey(Employee employee)
                : this(employee.LastName, employee.FirstName)
            {
            }
        }

        private class ImportItem
        {
            public string CurrentFirstName { get; set; }
            public string CurrentLastName { get; set; }
            public string RealFirstName { get; set; }
            public string RealLastName { get; set; }
            public string Email { get; set; }
            public string DomainUser { get; set; }
        }

        public override void Up()
        {
            var filePath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath)), "Import201210171646.json");
            var serializer = new Newtonsoft.Json.JsonSerializer();
            var data = serializer.Deserialize<ImportItem[]>(new Newtonsoft.Json.JsonTextReader(new StreamReader(filePath))).ToDictionary(x => new ImportKey(x));
           using (var session = DocumentStore.OpenSession())
            {
                var empty = false;
                var skip = 0;
                var take = 20;
                while (!empty) {
                    var result = session.Query<Employee>()
                        .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                        .Skip(skip)
                        .Take(take)
                        .ToArray();

                    foreach (var employee in result)
                    {
                        ImportItem item;
                        if (data.TryGetValue(new ImportKey(employee), out item))
                        {
                            if (!string.IsNullOrWhiteSpace(item.RealLastName))
                                employee.LastName = item.RealLastName;
                            if (!string.IsNullOrWhiteSpace(item.RealFirstName))
                                employee.FirstName = item.RealFirstName;
                            if (!string.IsNullOrWhiteSpace(item.Email))
                                employee.CorporativeEmail = item.Email;
                            if (!string.IsNullOrWhiteSpace(item.DomainUser))
                                employee.UserName = item.DomainUser;
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

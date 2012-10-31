using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    [Migration("201205220859", "Employee.Certification String => List<String>")]
    public class EmployeeCertificationsToList : Migration
    {
        const int PageSize = 64;

        public override void Up()
        {
            int start = 0;
            while (true)
            {
                var results = DocumentStore.DatabaseCommands.Query(
                    "Raven/DocumentsByEntityName",
                    new IndexQuery()
                    {
                        Query = "Tag:Employees",
                        PageSize = PageSize,
                        Start = start
                    },
                    null);


                if (results.Results.Count == 0)
                    break;

                foreach (var result in results.Results)
                {
                    var value = result["Certifications"];
                    if (value != null && value.Type == Newtonsoft.Json.Linq.JTokenType.String)
                    {
                        var str = value.ToString();
                        var list = str.Split(new[] { ';' }).Select(x => global::Raven.Json.Linq.RavenJObject.FromObject(new { Description = x.Trim() })).ToList();

                        DocumentStore.DatabaseCommands.Patch(
                            result["@metadata"].Value<string>("@id").ToString(),
                            new[]
                            {
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Unset,
                                    Name = "Certifications"
                                },
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Certifications",
                                    Value = new global::Raven.Json.Linq.RavenJArray(list)
                                }
                            });
                    }
                }

                start += PageSize;
            }
        }

        public override void Down()
        {
            int start = 0;
            while (true)
            {
                var results = DocumentStore.DatabaseCommands.Query(
                    "Raven/DocumentsByEntityName",
                    new IndexQuery()
                    {
                        Query = "Tag:Employees",
                        PageSize = PageSize,
                        Start = start
                    },
                    null);


                if (results.Results.Count == 0)
                    break;

                foreach (var result in results.Results)
                {
                    var value = result["Certifications"];
                    if (value != null && value.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {

                        var values = value.Values().Select(x => x.Value<string>("Description"));
                        var str = string.Join("; ", values);

                        DocumentStore.DatabaseCommands.Patch(
                            result["@metadata"].Value<string>("@id").ToString(),
                            new[]
                            {
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Unset,
                                    Name = "Certifications"
                                },
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Certifications",
                                    Value = new global::Raven.Json.Linq.RavenJValue(str)
                                }
                            });
                    }
                }

                start += PageSize;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Json.Linq;
using CommonJobs.Domain;

namespace CommonJobs.Migrations
{
    //TODO: Validate this cast => (Newtonsoft.Json.Linq.JTokenType)

    [Migration("201205011017", "Clean vacations string data")]
    public class CleanVacationsStringData : Migration
    {
        const string UpBackupKey = "MigrationBackup_Up_201205011017_CleanVacationsStringData";
        const string DownBackupKey = "MigrationBackup_Down_201205011017_CleanVacationsStringData";

        private void ForAllEmployees(Action<RavenJObject> action)
        {
            DoInResults(action, sortedBy: "Id", index: "Dynamic/Employees");
        }

        public override void Up()
        {
            ForAllEmployees(result =>
            {
                var value = result["Vacations"];
                if (value != null && ((Newtonsoft.Json.Linq.JTokenType)value.Type == Newtonsoft.Json.Linq.JTokenType.String || (Newtonsoft.Json.Linq.JTokenType)value.Type == Newtonsoft.Json.Linq.JTokenType.Null))
                {
                    var patchs = new List<PatchRequest>() {
                        new PatchRequest()
                        {
                            Type = PatchCommandType.Unset,
                            Name = "Vacations"
                        },
                        new PatchRequest()
                        {
                            Type = PatchCommandType.Modify,
                            Name = "@metadata",
                            Value = new RavenJObject(),
                            Nested = new []
                            {
                                new PatchRequest()
                                {
                                    Type = PatchCommandType.Set,
                                    Name = UpBackupKey,
                                    Value = value,
                                },
                                new PatchRequest()
                                {
                                    Type = PatchCommandType.Unset,
                                    Name = DownBackupKey,
                                }
                            }
                        }
                    };

                    var metadata = result["@metadata"] as RavenJObject;
                    RavenJToken newData;

                    if (metadata == null || !metadata.TryGetValue(DownBackupKey, out newData) || newData == null || (Newtonsoft.Json.Linq.JTokenType)newData.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                        newData = new global::Raven.Json.Linq.RavenJArray();

                    patchs.Add(new PatchRequest()
                    {
                        Type = PatchCommandType.Set,
                        Name = "Vacations",
                        Value = newData
                    });

                    DocumentStore.DatabaseCommands.Patch(
                        result["@metadata"].Value<string>("@id").ToString(),
                        patchs.ToArray());
                }
            });
        }

        public override void Down()
        {
            ForAllEmployees(result =>
            {
                var value = result["Vacations"];
                if (value == null || (Newtonsoft.Json.Linq.JTokenType)value.Type != Newtonsoft.Json.Linq.JTokenType.String)
                {
                    var patchs = new List<PatchRequest>() {
                        new PatchRequest()
                        {
                            Type = PatchCommandType.Unset,
                            Name = "Vacations"
                        },
                        new PatchRequest()
                        {
                            Type = PatchCommandType.Modify,
                            Name = "@metadata",
                            Value = new RavenJObject(),
                            Nested = new []
                            {
                                new PatchRequest()
                                {
                                    Type = PatchCommandType.Set,
                                    Name = DownBackupKey,
                                    Value = value,
                                },
                                new PatchRequest()
                                {
                                    Type = PatchCommandType.Unset,
                                    Name = UpBackupKey,
                                }
                            }
                        }
                    };

                    var metadata = result["@metadata"] as RavenJObject;
                    RavenJToken restoreData;
                    if (metadata != null && metadata.TryGetValue(UpBackupKey, out restoreData))
                    {
                        patchs.Add(new PatchRequest()
                        {
                            Type = PatchCommandType.Set,
                            Name = "Vacations",
                            Value = restoreData
                        });
                    }

                    DocumentStore.DatabaseCommands.Patch(
                        result["@metadata"].Value<string>("@id").ToString(),
                        patchs.ToArray());
                }
            });
        }
    }
}

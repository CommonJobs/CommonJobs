using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;
using CommonJobs.Utilities;

namespace CommonJobs.Migrations
{
    [Migration("201212280918", "Move applicant note type to event type")]
    public class MoveApplicantNoteTypeToEventType : Migration
    {
        private void ForAllApplicants(Action<RavenJObject> action)
        {
            DoInResults(action, sortedBy: "Id", index: "Dynamic/Applicants");
        }

        readonly Dictionary<string, string> noteTypeToEventTypeMap = new Dictionary<string, string>()
        {
            { "InteviewNote", ApplicantEventType.DefaultHRInterview },
            { "TechnicalInterviewNote", ApplicantEventType.DefaultTechnicalInterview }
        };

        
        public override void Up()
        {
            ForAllApplicants(result =>
            {
                var patchs = new List<PatchRequest>();
                if (result["Notes"].Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    var position = 0;
                    foreach (var note in result["Notes"].Values())
                    {
                        var noteType = note.Value<string>("NoteType");
                        var previousEventType = note.Value<string>("EventType");

                        if (previousEventType == null && noteType != null && noteTypeToEventTypeMap.ContainsKey(noteType))
                        {
                            patchs.Add(
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Modify,
                                    Name = "Notes",
                                    Position = position,
                                    Nested = new[] {
                                        new PatchRequest()
                                        {
                                            Type = PatchCommandType.Unset,
                                            Name = "NoteType"
                                        },
                                        new PatchRequest()
                                        {
                                            Type = PatchCommandType.Set,
                                            Name = "EventType",
                                            Value = noteTypeToEventTypeMap[noteType]
                                        },
                                        new PatchRequest()
                                        {
                                            Type = PatchCommandType.Set,
                                            Name = "EventTypeSlug",
                                            Value = noteTypeToEventTypeMap[noteType].GenerateSlug()
                                        }
                                    }
                                });
                        }

                        position++;
                    }

                    if (patchs.Any())
                    {
                        var metadata = result["@metadata"] as RavenJObject;
                        DocumentStore.DatabaseCommands.Patch(
                            result["@metadata"].Value<string>("@id").ToString(),
                            patchs.ToArray());
                    }
                }
            });
        }

        public override void Down()
        {
            var eventTypeToNoteTypeMap = noteTypeToEventTypeMap.ToDictionary(x => x.Value, x => x.Key);
            
            ForAllApplicants(result =>
            {
                var patchs = new List<PatchRequest>();
                if (result["Notes"].Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    var position = 0;
                    foreach (var note in result["Notes"].Values())
                    {
                        var eventType = note.Value<string>("EventType");
                        var previousNoteType = note.Value<string>("NoteType");

                        if (previousNoteType == null && eventType != null && eventTypeToNoteTypeMap.ContainsKey(eventType))
                        {
                            patchs.Add(
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Modify,
                                    Name = "Notes",
                                    Position = position,
                                    Nested = new[] {
                                        new PatchRequest()
                                        {
                                            Type = PatchCommandType.Unset,
                                            Name = "EventType"
                                        },
                                        new PatchRequest()
                                        {
                                            Type = PatchCommandType.Unset,
                                            Name = "EventTypeSlug"
                                        },
                                        new PatchRequest()
                                        {
                                            Type = PatchCommandType.Set,
                                            Name = "NoteType",
                                            Value = eventTypeToNoteTypeMap[eventType]
                                        }
                                    }
                                });
                        }
                        position++;
                    }

                    if (patchs.Any())
                    {
                        var metadata = result["@metadata"] as RavenJObject;
                        DocumentStore.DatabaseCommands.Patch(
                            result["@metadata"].Value<string>("@id").ToString(),
                            patchs.ToArray());
                    }
                }
            });
        }
    }
}

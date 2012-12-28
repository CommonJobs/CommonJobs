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

namespace CommonJobs.Migrations
{
    [Migration("201212280918", "Move applicant note type to event type")]
    public class MoveApplicantNoteTypeToEventType : Migration
    {
        private void ForAllApplicants(Action<RavenJObject> action)
        {
            DoInResults(action, sortedBy: "Id", index: "Dynamic/Applicants");
        }

        readonly Dictionary<int, string> noteTypeToEventTypeMap = new Dictionary<int, string>()
        {
            { 1, "Entrevista RRHH" },
            { 2, "Entrevista Técnica" }
        };

        
        public override void Up()
        {
            ForAllApplicants(result =>
            {
                var noteType = result.ContainsKey("NoteType") ? result.Value<int>("NoteType") : 0;
                var previousEventType = result.ContainsKey("EventType") ? result.Value<string>("EventType") : null;

                if (previousEventType == null && noteTypeToEventTypeMap.ContainsKey(noteType))
                {
                    var patchs = new List<PatchRequest>() {
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
                        }
                    };

                    var metadata = result["@metadata"] as RavenJObject;
                    DocumentStore.DatabaseCommands.Patch(
                        result["@metadata"].Value<string>("@id").ToString(),
                        patchs.ToArray());
                }
            });
        }

        public override void Down()
        {
            var eventTypeToNoteTypeMap = noteTypeToEventTypeMap.ToDictionary(x => x.Value, x => x.Key);
            
            ForAllApplicants(result =>
            {
                var eventType = result.ContainsKey("EventType") ? result.Value<string>("EventType") : null;
                var previousNoteType = result.ContainsKey("NoteType") ? result.Value<int>("NoteType") : 0;


                if (previousNoteType == 0 && eventTypeToNoteTypeMap.ContainsKey(eventType))
                {
                    var patchs = new List<PatchRequest>() {
                        new PatchRequest() 
                        {
                            Type = PatchCommandType.Unset,
                            Name = "EventType"
                        },
                        new PatchRequest() 
                        { 
                            Type = PatchCommandType.Set, 
                            Name = "NoteType", 
                            Value = eventTypeToNoteTypeMap[eventType]
                        }
                    };

                    var metadata = result["@metadata"] as RavenJObject;
                    DocumentStore.DatabaseCommands.Patch(
                        result["@metadata"].Value<string>("@id").ToString(),
                        patchs.ToArray());
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Json.Linq;

namespace CommonJobs.Migrations
{
    [Migration("201301161535", "Clean Last-Modified-By to fix possible issues")]
    public class RemoveDomainUsers : Migration
    {
        public override void Up()
        {
            var patchs = new List<PatchRequest>() {
                new PatchRequest() 
                { 
                    Type = PatchCommandType.Modify, 
                    Name = "@metadata", 
                    Value = new RavenJObject(), 
                    Nested = new []
                    { 
                        new PatchRequest() 
                        { 
                            Type = PatchCommandType.Unset, 
                            Name = "Last-Modified-By"
                        }
                    }
                }
            };

            DocumentStore.DatabaseCommands.UpdateByIndex("Raven/DocumentsByEntityName",
                                                         new IndexQuery {  },
                                                         patchs.ToArray(), 
                                                         allowStale: true);

        }

        public override void Down()
        {
        }

    }
}

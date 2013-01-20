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
    [Migration("201301163531", "Delete Menu because it could be broken")]
    public class DeleteMenu : Migration
    {
        public override void Up()
        {
            DocumentStore.DatabaseCommands.Delete("Menu/DefaultMenu", null);
        }

        public override void Down()
        {
        }

    }
}

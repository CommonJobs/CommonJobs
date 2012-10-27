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
    [Migration("201208250850", "Resave all persons in order to Add Slot information to AllAttchments field")]
    public class RefactorAllAttachmentData : Migration
    {
        public override void Up()
        {
            ReSaveAll<Employee, string>(x => x.Id);
            ReSaveAll<Applicant, string>(x => x.Id);
        }

        public override void Down()
        {
            Up();
        }

    }
}

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
    [Migration("201212310827", "Resave all applicants in order to update note slugs")]
    public class ReStoreApplicants : Migration
    {
        public override void Up()
        {
            ReSaveAll<Applicant, string>(x => x.Id);
        }

        public override void Down()
        {
            Up();
        }

    }
}

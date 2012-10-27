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
    [Migration("201208251121", "Move cv attachments to CV slot")]
    public class MoveCVAttachmentsToSlot : MoveAttachmentToSlotBaseMigration
    {
        protected override string DownNoteText
        {
            get { return "Adjunto movido desde el slot de CV"; }
        }

        protected override string SlotId
        {
            get { return "AttachmentSlots/Employee/CV"; }
        }

        protected override string[] UpSearchTexts
        {
            get { return new [] { "CV", "Professional Experience", "Nacionalidad", "Curriculum" }; }
        }
    }
}

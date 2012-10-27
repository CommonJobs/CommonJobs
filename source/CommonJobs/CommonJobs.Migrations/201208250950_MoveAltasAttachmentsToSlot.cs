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
    [Migration("201208250950", "Move altas attachments to Altas slot")]
    public class MoveAltasAttachmentsToSlot : MoveAttachmentToSlotBaseMigration
    {
        protected override string DownNoteText
        {
            get { return "Adjunto movido desde el slot de Altas"; }
        }

        protected override string SlotId
        {
            get { return "AttachmentSlots/Employee/Altas"; }
        }

        protected override string[] UpSearchTexts
        {
            get { return new[] { "Alta Original para el empleador" }; }
        }
    }
}

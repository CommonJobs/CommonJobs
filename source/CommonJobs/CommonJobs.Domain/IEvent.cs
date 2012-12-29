using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommonJobs.Domain
{
    public interface IEvent
    {
        [Display(Name = "Fecha Real")]
        [DataType(DataType.DateTime)]
        DateTime RealDate { get; set; }

        [Display(Name = "Fecha registrada")]
        [DataType(DataType.DateTime)]
        DateTime RegisterDate { get; set; }

        [Display(Name = "Nota")]
        [DataType(DataType.MultilineText)]
        string Note { get; set; }

        string EventType { get; }
    }

    public interface IEventWithAttachment : IEvent
    {
        AttachmentReference Attachment { get; set; }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class Absence : IEventWithAttachment
    {
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        public DateTime RealDate { get; set; }

        [Display(Name = "Fecha registrada")]
        [DataType(DataType.DateTime)]
        public DateTime RegisterDate { get; set; }

        [Display(Name = "Nota")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }

        public AttachmentReference Attachment { get; set; }

        public string EventType { get { return "_Absence_"; } }

        public string EventTypeSlug { get { return EventType.GenerateSlug(); } }

        public string Reason { get; set; }
        
        public DateTime? To { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public AbsenceType AbsenceType { get; set; }
        public bool HasCertificate { get; set; }

        public string ReasonSlug
        {
            get { return Reason.GenerateSlug(); }
        }

    }
}

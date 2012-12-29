using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public string EventType { get { return "[Absence]"; } }

        public string Reason { get; set; }
        
        public DateTime? To { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public AbsenceType AbsenceType { get; set; }
        public bool HasCertificate { get; set; }

        static Regex notAllowed = new Regex("[^a-z0-9]+");
        static Regex trim = new Regex("^-|-$");
        //TODO: refactor
        public string ReasonSlug
        {
            get { return Reason == null ? null : trim.Replace(notAllowed.Replace(Reason.ToLower(), "-"), ""); }
        }
        //From Event:
        //  public DateTime RealDate { get; set; }
        //  public DateTime RegisterDate { get; set; }
        //  public string Note { get; set; }

    }
}

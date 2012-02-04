using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CommonJobs.Domain
{
    public class DatedNote
    {
        [Display(Name = "Fecha")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Display(Name = "Nota")]
        [DataType(DataType.MultilineText)]
        public string Note { get; set; }
    }
}
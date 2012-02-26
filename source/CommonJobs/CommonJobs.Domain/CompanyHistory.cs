using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CommonJobs.Domain
{
    public class CompanyHistory
    {
        [Display(Name="Nombre de la compañía")]
        public string CompanyName { get; set; }

        [Display(Name = "Actual")]
        public bool IsCurrent { get; set; }

        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Fecha de fin")]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }
    }
}

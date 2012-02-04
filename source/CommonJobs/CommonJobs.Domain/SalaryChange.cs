using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CommonJobs.Domain
{
    public class SalaryChange : Happening
    {
        [Display(Name = "Salary")]
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
    }
}

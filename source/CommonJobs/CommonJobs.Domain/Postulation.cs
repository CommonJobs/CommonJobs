using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class Postulation
    {
        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Teléfono")]
        public string Phone { get; set; }
        
        [Display(Name = "Currículum")]
        public TemporalFileReference Curriculum { get; set; }
    }
}

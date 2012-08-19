using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class Postulation
    {
        public string JobSearchId { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Requerido")]
        [Display(Name = "Currículum")]
        public TemporalFileReference Curriculum { get; set; }

        [Display(Name = "URL de perfil de LinkedIn")]
        public string LinkedInUrl { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Comentarios")]
        public string Comment { get; set; }
    }
}

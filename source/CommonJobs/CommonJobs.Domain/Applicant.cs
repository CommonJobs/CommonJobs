using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CommonJobs.Domain
{
    //TODO could this and Employee be refactored into a Person base class? How will be RavenDB affected?
    public class Applicant
    {
        public string Id { get; set; }

        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Display(Name = "Teléfonos")]
        public string Telephones { get; set; }

        [Display(Name = "Estado civil")]
        public MaritalStatus MaritalStatus { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Compañías")]
        public List<CompanyHistory> CompanyHistory { get; set; }

        [Display(Name = "Idioma")]
        public string EnglishLevel { get; set; }

        [Display(Name = "Universidad")]
        public string College { get; set; }
        
        [Display(Name = "Título")]
        public string Degree { get; set; }
        
        [Display(Name = "Recibido")]
        public bool IsGraduated { get; set; }

        //TODO attachments?

        //TODO this should be a collection of "tags"
        //TODO this should be a collection of pre-defined values. Each of these should be associated with a tag. Example: Skill C#, Skill Level Advanced. Skill HTML, Skill Level Medium.
        [Display(Name = "Skills")]
        //Only in detailed view
        public string Skills { get; set; }

        [Display(Name = "Notas")]
        public List<SimpleNote> Notes { get; set; }

        [Display(Name = "Resaltado")]
        public bool IsHighlighted { get; set; }
    }
}

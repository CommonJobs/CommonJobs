using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace CommonJobs.Domain
{
    public class Person
    {
        public Person() { }

        public Person(string name): this()
        {
            if (!string.IsNullOrWhiteSpace(name))
                SetName(name);
        }

        private void SetName(string name)
        {
            //TODO unify this with the javascript logic
            var parts = name.Split(new[] { ',' }, 2);
            if (parts.Length > 1)
            {
                LastName = parts[0].Trim();
                FirstName = parts[1].Trim();
            }
            else
            {
                parts = name.Split(new[] { ' ' }, 2);
                if (parts.Length > 1)
                {
                    LastName = parts[1].Trim();
                    FirstName = parts[0].Trim();
                }
                else
                {
                    FirstName = name.Trim();
                }
            }
		}
		
        public string Id { get; set; }

        public ImageAttachment Photo { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; }

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

        [Display(Name = "Idioma")]
        public string EnglishLevel { get; set; }

        [Display(Name = "Universidad")]
        public string College { get; set; }

        [Display(Name = "Título")]
        public string Degree { get; set; }

        [Display(Name = "Recibido")]
        public bool IsGraduated { get; set; }

        //TODO this should be a collection of "tags"
        //TODO this should be a collection of pre-defined values. Each of these should be associated with a tag. Example: Skill C#, Skill Level Advanced. Skill HTML, Skill Level Medium.
        [Display(Name = "Skills")]
        //Only in detailed view
        public string Skills { get; set; }

        public virtual IEnumerable<SlotWithAttachment> AllAttachmentReferences
        {
            get { return SlotWithAttachment.GenerateFromImage(Photo); }
        }
    }
}

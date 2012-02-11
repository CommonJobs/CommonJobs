using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class Employee
    {
        //[HiddenInput]
        public string Id { get; set; }

        [Display(Name = "Nombre")]
        public string FirstName { get; set; }

        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Display(Name = "Dirección")]
        public string Address { get; set; }

        [Display(Name = "Teléfonos")]
        public string Telephones { get; set; }

        [Display(Name = "Estado Civil")]
        public MaritalStatus MaritalStatus { get; set; }
        
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.Date)]
        //Only in detailed view
        public DateTime BirthDate { get; set; }

        [Display(Name = "Rem. Inicial")]
        [DataType(DataType.Currency)]
        public Decimal InitialSalary 
        {
            get
            {
                return SalaryChanges
                    .EmptyIfNull()
                    .OrderBy(x => x.RealDate)
                    .Select(x => (decimal?)x.Salary)
                    .FirstOrDefault() ?? 0;
            } 
        }

        [Display(Name = "Rem.Actual")]
        [DataType(DataType.Currency)]
        public Decimal CurrentSalary 
        {
            get
            {
                return SalaryChanges
                    .EmptyIfNull()
                    .OrderByDescending(x => x.RealDate)
                    .Select(x => (decimal?)x.Salary)
                    .FirstOrDefault() ?? 0;
            } 
        }

        [Display(Name = "Horario")]
        public string Schedule { get; set; }

        //TODO this should be auto-calculated from Schedule
        [Display(Name = "Carga horaria")]
        //Only in detailed view
        public decimal WorkingHours { get; set; }
        
        [Display(Name = "Legajo")]
        public string FileId { get; set; }
        
        [Display(Name = "Almuerzo")]
        public bool Lunch { get; set; }
        
        [Display(Name = "Banco")]
        public string BankAccount { get; set; }
        
        [Display(Name = "Obra Social")]
        public string HealthInsurance { get; set; }

        [Display(Name = "Plataforma")]
        public string Platform { get; set; }

        //TODO this should be a collection of "tags"
        //TODO this should be a collection of pre-defined values. Each of these should be associated with a tag. Example: Skill C#, Skill Level Advanced. Skill HTML, Skill Level Medium.
        [Display(Name = "Skills")]
        //Only in detailed view
        public string Skills { get; set; }

        [Display(Name = "Certificaciones")]
        //Only in detailed view
        public string Certifications { get; set; }

        [Display(Name = "Universidad")]
        //Only in detailed view
        public string College { get; set; }

        [Display(Name = "Título")]
        //Only in detailed view
        public string Degree { get; set; }

        [Display(Name = "Recibido")]
        //Only in detailed view
        public bool IsGraduated { get; set; }

        [Display(Name = "Posición Inicial")]
        //Only in detailed view
        public string InitialPosition { get; set; }

        [Display(Name = "Posición Actual")]
        public string CurrentPosition { get; set; }

        [Display(Name = "Seniority")]
        //Only in detailed view
        public string Seniority { get; set; }

        [Display(Name = "Idioma")]
        //Only in detailed view
        public string EnglishLevel { get; set; }

        [Display(Name = "Proyecto")]
        public string CurrentProject { get; set; }

        [Display(Name = "Convenio")]
        public string Agreement { get; set; }

        //TODO this should be a collection of start-end dates
        [Display(Name = "Vacaciones")]
        //Only in detailed view
        public string Vacations { get; set; }

        [Display(Name = "Observaciones")]
        //Only in detailed view
        public List<SimpleNote> Notes { get; set; }

        [Display(Name = "Historial de sueldos")]
        public List<SalaryChange> SalaryChanges { get; set; }

        //private IEnumerable<T> TakeHappenings<T>() where T : Happening
        //{
        //    return Happenings.EmptyIfNull().OfType<T>().Where(x => !x.Deleted).OrderByDescending(x => x.RealDate);
        //}

        [Display(Name = "URL de Imagen")]
        public string PictureUrl { get; set; }

        public IEnumerable<EmployeeEvent> Events 
        {
            get
            {
                return Notes.EmptyIfNull().Cast<EmployeeEvent>()
                    .Union(SalaryChanges.EmptyIfNull().Cast<EmployeeEvent>())
                    .OrderByDescending(x => x.RealDate)
                    .ThenBy(x => x.RegisterDate);
            }
        }
    }
}
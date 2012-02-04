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
        
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime WorkingSince { get; set; }

        [Display(Name = "Fecha Nacimiento")]
        [DataType(DataType.Date)]
        //Only in detailed view
        public DateTime BirthDate { get; set; }
        
        [Display(Name = "Rem.Inicial")]
        [DataType(DataType.Currency)]
        public Decimal InitialSalary { get; set; }

        [Display(Name = "Rem.Actual")]
        [DataType(DataType.Currency)]
        public Decimal CurrentSalary 
        { 
            get 
            {
                return SalaryChanges
                    .Select(x => (decimal?)x.Salary)
                    .FirstOrDefault() ?? InitialSalary; 
            } 
        }

        [Display(Name = "Horario")]
        public string Schedule { get; set; }

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

        [Display(Name = "Skills")]
        //Only in detailed view
        public string Skills { get; set; }

        [Display(Name = "Nivel")]
        //Only in detailed view
        public string SkillLevel { get; set; }

        [Display(Name = "Certificación")]
        //Only in detailed view
        public string Certifications { get; set; }

        [Display(Name = "Universidad")]
        //Only in detailed view
        public string University { get; set; }

        [Display(Name = "Título")]
        //Only in detailed view
        public string Title { get; set; }

        [Display(Name = "Recibido")]
        //Only in detailed view
        public bool Received { get; set; }

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
        public string CurrentProyect { get; set; }

        [Display(Name = "Convenio")]
        public string Agreement { get; set; }

        [Display(Name = "Vacaciones")]
        //Only in detailed view
        public string Vacations { get; set; }

        [Display(Name = "Comentarios")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Observaciones")]
        //Only in detailed view
        public IEnumerable<SimpleNote> Notes
        {
            get { return TakeHappenings<SimpleNote>(); }
        }

        [Display(Name = "Historial de sueldos")]
        public IEnumerable<SalaryChange> SalaryChanges
        {
            get { return TakeHappenings<SalaryChange>(); }
        }

        private IEnumerable<T> TakeHappenings<T>() where T : Happening
        {
            return Happenings.EmptyIfNull().OfType<T>().Where(x => !x.Deleted).OrderByDescending(x => x.RealDate);
        }

        public List<Happening> Happenings { get; set; }
    }
}
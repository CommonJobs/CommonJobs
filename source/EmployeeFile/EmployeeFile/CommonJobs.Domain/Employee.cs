using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Domain
{
    public class Employee
    {
        [HiddenInput]
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
        public Decimal InitialRemuneration { get; set; }
        
        [Display(Name = "Ajuste")]
        [DataType(DataType.Currency)]
        public Decimal CurrentRemuneration { get; set; }
        
        [Display(Name = "Horario")]
        public string Schedule { get; set; }

        [Display(Name = "Carga horaria")]
        //Only in detailed view
        public decimal WorkingHours { get; set; }
        
        //CENSORED
        
        [Display(Name = "Legajo")]
        public string FileId { get; set; }
        
        [Display(Name = "Almuerzo")]
        public string Lunch { get; set; }
        
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
        public string Received { get; set; }

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

        [Display(Name = "Aumentos")]
        public string SalaryIncreases { get; set; }

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
        public List<DatedNote> Notes { get; set; }
    }
}
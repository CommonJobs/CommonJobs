using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class Employee: Person
    {
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime? HiringDate { get; set; }

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
        public decimal? WorkingHours { get; set; }
        
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

        [Display(Name = "Certificaciones")]
        //Only in detailed view
        public string Certifications { get; set; }

        [Display(Name = "Posición Inicial")]
        //Only in detailed view
        public string InitialPosition { get; set; }

        [Display(Name = "Posición Actual")]
        public string CurrentPosition { get; set; }

        [Display(Name = "Seniority")]
        //Only in detailed view
        public string Seniority { get; set; }

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
        public List<NoteWithAttachment> Notes { get; set; }

        [Display(Name = "Historial de sueldos")]
        public List<SalaryChange> SalaryChanges { get; set; }

        ////It is not nessesary now, I was testing something
        //public IEnumerable<Event> Events 
        //{
        //    get
        //    {
        //        return Notes.EmptyIfNull().Cast<Event>()
        //            .Union(SalaryChanges.EmptyIfNull().Cast<Event>())
        //            .OrderByDescending(x => x.RealDate)
        //            .ThenBy(x => x.RegisterDate);
        //    }
        //}


        public override IEnumerable<AttachmentReference> AllAttachmentReferences
        {
            get { return base.AllAttachmentReferences.Union(Notes.EmptyIfNull().Select(x => x.Attachment)).Where(x => x != null); }
        }
    }
}
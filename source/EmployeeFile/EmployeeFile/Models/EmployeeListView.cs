using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeFile.Models
{
    public class EmployeeListView
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

        [Display(Name = "Rem.Inicial")]
        [DataType(DataType.Currency)]
        public Decimal InitialRemuneration { get; set; }

        [Display(Name = "Ajuste")]
        [DataType(DataType.Currency)]
        public Decimal CurrentRemuneration { get; set; }

        [Display(Name = "Horario")]
        public string Schedule { get; set; }

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

        [Display(Name = "Posición Actual")]
        public string CurrentPosition { get; set; }

        [Display(Name = "Aumentos")]
        public string SalaryIncreases { get; set; }

        [Display(Name = "Proyecto")]
        public string CurrentProyect { get; set; }

        [Display(Name = "Convenio")]
        public string Agreement { get; set; }

        [Display(Name = "Comentarios")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }
    }
}
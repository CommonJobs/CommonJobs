using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using System.Text.RegularExpressions;
using CommonJobs.Utilities;

namespace CommonJobs.Application.EmployeeAbsences
{
    public class AbsencesSearchResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? HiringDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public List<AbsenceResult> Absences { get; set; }

        public AbsencesSearchResult(Employee employee, IEnumerable<Absence> absences, IEnumerable<Vacation> vacations)
        {
            Id = employee.Id;
            FirstName = employee.FirstName;
            LastName = employee.LastName;
            HiringDate = employee.HiringDate;
            TerminationDate = employee.TerminationDate;
            Absences = absences.Select(x => new AbsenceResult(x)).Union(vacations.Select(x => new AbsenceResult(x))).ToList();
        }
    }

    public class AbsenceResult
    {
        public string Reason { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string AbsenceType { get; set; }
        public bool HasCertificate { get; set; }
        public string ReasonSlug { get; set; }
        public AttachmentReference Attachment { get; set; }
        public string Note { get; set; }

        public AbsenceResult(Vacation vacation)
        {
            Reason = "Vacaciones";
            ReasonSlug = "vacaciones";
            AbsenceType = "Vacations";
            HasCertificate = false;
            From = vacation.From;
            To = vacation.To;
            Attachment = null;
        }

        public AbsenceResult(Absence absence)
        {
            Reason = absence.Reason;
            ReasonSlug = absence.ReasonSlug;
            AbsenceType = absence.AbsenceType.ToString();
            HasCertificate = absence.HasCertificate;
            From = absence.RealDate;
            To = absence.To ?? absence.RealDate;
            Attachment = absence.Attachment;
            Note = absence.Note;
        }
    }
}
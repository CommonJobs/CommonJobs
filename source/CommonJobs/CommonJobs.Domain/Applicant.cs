using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    //TODO could this and Employee be refactored into a Person base class? How will be RavenDB affected?
    public class Applicant : Person, IShareableEntity
    {
        public Applicant(): base() { }

        public Applicant(string name) : base(name) { }

        [Display(Name = "Compañías")]
        public List<CompanyHistory> CompanyHistory { get; set; }

        [Display(Name = "Resaltado")]
        public bool IsHighlighted { get; set; }

        [Display(Name = "Notas")]
        public List<NoteWithAttachment> Notes { get; set; }

        [Display(Name = "LinkedIn")]
        public string LinkedInLink { get; set; }

        public void AddNote(NoteWithAttachment note)
        {
            this.Notes.Add(note);
        }
        public void AddGeneralNote(string note, AttachmentReference attachment = null)
        {
            AddNote(new NoteWithAttachment()
            {
                Note = note,
                RealDate = DateTime.Now,
                RegisterDate = DateTime.Now,
                Attachment = attachment
            });
        }

        public override IEnumerable<SlotWithAttachment> AllAttachmentReferences
        {
            get { return base.AllAttachmentReferences.Union(SlotWithAttachment.GenerateFromNotes(Notes)); }
        }

        //TODO: automatically remove expired links
        public SharedLinkList SharedLinks { get; set; }

        public string JobSearchId { get; set; }

        public string EmployeeId { get; set; }

        public bool IsHired
        {
            get { return !string.IsNullOrWhiteSpace(EmployeeId); }
        }

        public Employee Hire(DateTime hiringDate)
        {
            var employee = new Employee();
            employee.HiringDate = hiringDate;

            //It is not sooo elegant but works
            employee.IdChanged += (sender, e) =>
            {
                EmployeeId = employee.Id;
            };

            employee.FirstName = FirstName;
            employee.LastName = LastName;
            employee.Address = Address;
            employee.ApplicantId = Id;
            employee.BirthDate = BirthDate;
            employee.College = College;
            employee.Degree = Degree;
            employee.Email = Email;
            employee.EnglishLevel = EnglishLevel;
            employee.IsGraduated = IsGraduated;
            employee.MaritalStatus = MaritalStatus;
            employee.Skills = Skills;
            employee.TechnicalSkills = TechnicalSkills;
            employee.Telephones = Telephones;
            //employee.Photo = Photo //I am not sure because of entity associated to attachments

            return employee;
        }
    }
}

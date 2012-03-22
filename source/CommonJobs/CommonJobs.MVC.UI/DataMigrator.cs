using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using CommonJobs.Domain;

namespace CommonJobs.Mvc.UI
{
    public class DataMigrator : IDisposable
    {
        public class VersionDocument
        {
            public int DataVersion { get; set; }
        }

        IDocumentStore store;
        Lazy<IDocumentSession> lazySession;
        IDocumentSession Session { get { return lazySession.Value; } }

        public DataMigrator(IDocumentStore store)
        {
            this.store = store;
            lazySession = new Lazy<IDocumentSession>(store.OpenSession);
        }

        private int current = -1;
        private int step = 1;

        public void UpdateStep(Action action)
        {
            if (current < 0)
                current = GetDataVersion();
            if (current < step) 
            {
                if (action != null)
                {
                    action();
                }
                SetDataVersion(step);
            }
            step++;
        }

        public void Execute()
        {
            current = -1;
            step = 1;
            //Do not remove or alter the order
            UpdateStep(CreateDataVersionDocument);
            UpdateStep(CreateSampleEmployeeData);
            UpdateStep(CreateSampleApplicantdata);
            UpdateStep(CreateFirstUser);
            //Insert new actions here
            Session.SaveChanges();
        }

        private void CreateDataVersionDocument()
        {
            var document = new VersionDocument();
            document.DataVersion = 0;
            Session.Store(document, "DataVersionDocument");
        }

        private int GetDataVersion()
        {
            var document = Session.Load<VersionDocument>("DataVersionDocument");
            if (document == null)
                return 0;
            else
                return document.DataVersion;
        }

        private void SetDataVersion(int version)
        {
            var document = Session.Load<VersionDocument>("DataVersionDocument");
            document.DataVersion = version;
        }

        private void CreateSampleApplicantdata()
        {
            var applicant = new Applicant()
            {
                FirstName = "Fulano",
                LastName = "De Tal",
                Address = "Calle Falsa 123, 2do piso (vecino de Andrés)",
                Telephones = "0223-12345678, 155-208557",
                MaritalStatus = CommonJobs.Domain.MaritalStatus.Single,
                Email = "fulanito@gmail.com",
                CompanyHistory = new List<CompanyHistory>()
                {
                    new CompanyHistory() {
                        CompanyName = "Google Inc.",
                        IsCurrent = true,
                        StartDate = DateTime.Parse("2007-01-01"),
                        EndDate = null
                    }
                },
                Degree = "CS Master",
                College = "MIT",
                IsGraduated = true,
                IsHighlighted = true,
                Notes = new List<ApplicantNote>()
                {
                    new ApplicantNote() {
                        Note = "Parece muy buena opción, su experiencia previa es increíble.",
                        RealDate = DateTime.Parse("2012-02-15"), 
                        RegisterDate = DateTime.Parse("2012-02-15"),
                        NoteType = ApplicantNoteType.InteviewNote
                    },
                    new ApplicantNote() {
                        Note = "Demostró tener amplios conocimientos de project management.",
                        RealDate = DateTime.Parse("2012-02-16"),
                        RegisterDate = DateTime.Parse("2012-03-01"),
                        NoteType = ApplicantNoteType.TechnicalInterviewNote
                    },
                    new ApplicantNote() {
                        Note = "Tiene pensado mudarse a Mar del Plata.",
                        RealDate = DateTime.Parse("2012-02-16"),
                        RegisterDate = DateTime.Parse("2012-02-16"),
                        NoteType = ApplicantNoteType.GeneralNote
                    }
                },
                Skills = "C#, Python, Ruby, Perl, HTML, CSS, HTML5, JS, SEO"
            };

            Session.Store(applicant);
        }


        private void CreateFirstUser()
        {
            var user = new User("admin", "admin");
            Session.Store(user);
        }


        private void CreateSampleEmployeeData()
        {
            var employee = new CommonJobs.Domain.Employee()
            {
                FirstName = "Juan",
                LastName = "Perez",
                Address = "Calle Falsa 123",
                Agreement = "Empleados de Comercio",
                BankAccount = "Francés",
                BirthDate = DateTime.Parse("1978-12-02"),
                College = "FASTA",
                CurrentPosition = "Senior Developer",
                CurrentProject = "Shop and Compare",
                Degree = "Ingeniero en Informática",
                EnglishLevel = "Medio",
                FileId = "1234567",
                HealthInsurance = "Swiss Medical",
                HiringDate = DateTime.Parse("2010-07-07"),
                InitialPosition = "Junior Developer",
                Notes = new List<CommonJobs.Domain.NoteWithAttachment>()
                {
                    new CommonJobs.Domain.NoteWithAttachment() 
                    {
                        RealDate = DateTime.Parse("2011-07-07"), 
                        RegisterDate = DateTime.Parse("2011-07-08"),
                        Note = "Trajo una torta para festejar su primer año en la empresa"
                    }
                },
                Platform = "Mar del Plata",
                SalaryChanges = new List<CommonJobs.Domain.SalaryChange>()
                {
                    new CommonJobs.Domain.SalaryChange() 
                    {
                        RealDate = DateTime.Parse("2011-07-09"), 
                        RegisterDate = DateTime.Parse("2011-07-09"),
                        Note = "Como la torta estaba muy buena se decidio aumentarle el sueldo",
                        Salary = 1500
                    },
                    new CommonJobs.Domain.SalaryChange() 
                    {
                        RealDate = DateTime.Parse("2011-09-09"), 
                        RegisterDate = DateTime.Parse("2011-09-09"),
                        Note = "Se lo merece!",
                        Salary = 2000
                    }
                },
                Schedule = "Lunes a Viernes desde las 9hs",
                Seniority = "Senior",
                Skills = "C#, Javascript",
                WorkingHours = 40
            };
            Session.Store(employee);
        }

        private bool ThereIsNoEmployeeData()
        {
            return !Session.Query<CommonJobs.Domain.Employee>().Any();
        }

        private bool ThereIsNoApplicantData()
        {
            return !Session.Query<CommonJobs.Domain.Applicant>().Any();
        }

        public void Dispose()
        {
            if (lazySession != null && lazySession.IsValueCreated)
                Session.Dispose();
            lazySession = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Raven.Migrations;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Json.Linq;
using CommonJobs.Domain;

namespace CommonJobs.Migrations
{
    [Migration("201204270835", "Fill database with sample data")]
    public class InitialData : Migration
    {
        public override void Up()
        {
            using (var session = DocumentStore.OpenSession())
            {
                var thereAreUsers = session.Query<User>().Any();
                if (!thereAreUsers)
                {
                    var user = new User("admin", "admin");
                    session.Store(user);
                }

                var thereAreApplicants = session.Query<Applicant>().Any();
                if (!thereAreApplicants)
                {
                    var applicant = GenerateSampleApplicant();
                    session.Store(applicant);
                }

                var thereAreEmployees = session.Query<Employee>().Any();
                if (!thereAreEmployees)
                {
                    var employee = GenerateSampleEmployee();
                    session.Store(employee);
                }

                session.SaveChanges();
            }
        }

        private static Applicant GenerateSampleApplicant()
        {
            return new Applicant()
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
        }

        private static Employee GenerateSampleEmployee()
        {
            return new Employee()
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
        }

        public override void Down()
        {
        }
    }
}

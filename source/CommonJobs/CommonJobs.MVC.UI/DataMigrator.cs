﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using CommonJobs.Domain;

namespace CommonJobs.MVC.UI
{
    public class DataMigrator : IDisposable
    {
        IDocumentStore store;
        Lazy<IDocumentSession> lazySession;
        IDocumentSession Session { get { return lazySession.Value; } }

        public DataMigrator(IDocumentStore store)
        {
            this.store = store;
            lazySession = new Lazy<IDocumentSession>(store.OpenSession);
        }


        public void Execute()
        {
            if (ThereIsNoEmployeeData())
                CreateSampleEmployeeData();

            if (ThereIsNoApplicantData())
                CreatesampleApplicantdata();
        }

        private void CreatesampleApplicantdata()
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
                Notes = new List<SimpleNote>()
                {
                    new SimpleNote() {
                        Note = "Parece muy buena opción, su experiencia previa es increíble.",
                        RealDate = DateTime.Parse("2012-02-15"), 
                        RegisterDate = DateTime.Parse("2012-02-15")
                    }
                },
                Skills = "C#, Python, Ruby, Perl, HTML, CSS, HTML5, JS, SEO"
            };

            Session.Store(applicant);
            Session.SaveChanges();
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
                Notes = new List<CommonJobs.Domain.SimpleNote>()
                {
                    new CommonJobs.Domain.SimpleNote() 
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
            Session.SaveChanges();
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
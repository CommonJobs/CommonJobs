using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System.Globalization;
using CommonJobs.Domain;

namespace CommonJobs.Infrastructure.EmployeeSearching
{
    public class Employee_QuickSearch : AbstractMultiMapIndexCreationTask<Employee_QuickSearch.Projection>
    {
        //TODO: Mejorar esto. Está realmente feo y llena el indice con archivos repetidos, 
        //pero es la única forma de hacerlo funcionar que encontré hasta ahora

        public class OrphanAttachment
        {
            public string Id { get; set; }
            public string FileName { get; set; }
            public string PlainContent { get; set; }
        }

        public class Projection
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Skills { get; set; }
            public string FullName1 { get; set; }
            public string FullName2 { get; set; }
            public string Terms { get; set; }
            public string FileId { get; set; }
            public string Platform { get; set; }
            public string CurrentPosition { get; set; }
            public string CurrentProject { get; set; }
            public string Notes { get; set; }
            public ImageAttachment Photo { get; set; } 

            public string[] AttachmentIds { get; set; }
            public string[] AttachmentNames { get; set; }
            public string[] AttachmentContent { get; set; }

            public bool IsEmployee { get; set; }
            public OrphanAttachment[] OrphanAttachments { get; set; }
        }
        
        public Employee_QuickSearch()
		{
            AddMap<Employee>(employees => 
                from employee in employees
                select new 
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Skills = employee.Skills,
                    FullName1 = string.Format("{0}, {1}", employee.LastName, employee.FirstName),
                    FullName2 = string.Format("{0} {1}", employee.FirstName, employee.LastName),
                    AttachmentIds = employee.AllAttachmentReferences.Select(x => x.Id).ToArray(),
                    AttachmentNames = employee.AllAttachmentReferences.Select(x => x.FileName).ToArray(),
                    AttachmentContent = new string[0],
                    IsEmployee = true,
                    OrphanAttachments = new dynamic[0], 
                    FileId = employee.FileId,
                    Platform = employee.Platform,
                    CurrentPosition = employee.CurrentPosition,
                    CurrentProject = employee.CurrentProject,
                    Notes = employee.Notes,
                    Photo = employee.Photo,
                    Terms = new object[]
                    {
                        string.Format("{0:yyyy-MM-dd}", employee.HiringDate),
                        string.Format("{0:dd-MM-yyyy}", employee.HiringDate),
                        string.Format("{0:MM-dd-yyyy}", employee.HiringDate),
                        string.Format("{0:MMMM}", employee.HiringDate),
                        string.Format("{0:yyyy-MM-dd}", employee.BirthDate),
                        string.Format("{0:dd-MM-yyyy}", employee.BirthDate),
                        string.Format("{0:MM-dd-yyyy}", employee.BirthDate),
                        string.Format("{0:MMMM}", employee.BirthDate),
                        employee.BankAccount,
                        employee.HealthInsurance,
                        employee.EnglishLevel,
                        employee.Seniority,
                        employee.Degree,
                        employee.College
                    }
                });
            Indexes.Add(x => x.FirstName, FieldIndexing.Analyzed);
            Indexes.Add(x => x.LastName, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Skills, FieldIndexing.Analyzed);
            Indexes.Add(x => x.AttachmentNames, FieldIndexing.Analyzed);
            Indexes.Add(x => x.AttachmentContent, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Platform, FieldIndexing.Analyzed);
            Indexes.Add(x => x.CurrentPosition, FieldIndexing.Analyzed);
            Indexes.Add(x => x.CurrentProject, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Notes, FieldIndexing.Analyzed);
            Indexes.Add(x => x.Terms, FieldIndexing.Analyzed);
		}
    }
}
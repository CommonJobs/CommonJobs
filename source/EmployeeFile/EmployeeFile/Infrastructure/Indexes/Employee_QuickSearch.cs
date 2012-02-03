using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeFile.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System.Globalization;

namespace EmployeeFile.Infrastructure.Indexes
{
    public class Employee_QuickSearch : AbstractIndexCreationTask<Employee, Employee_QuickSearch.Query>
    {
        public class Query
        {
            public string ByTerm { get; set; }
        }

        public Employee_QuickSearch()
		{
            Map = employees => from employee in employees
                               select new 
                               {
                                   ByTerm = new object[]
                                   {
                                       employee.FirstName,
                                       employee.LastName,
                                       string.Format("{0:yyyy-MM-dd}", employee.WorkingSince),
                                       string.Format("{0:dd-MM-yyyy}", employee.WorkingSince),
                                       string.Format("{0:MM-dd-yyyy}", employee.WorkingSince),
                                       string.Format("{0:MMMM}", employee.WorkingSince),
                                       string.Format("{0:yyyy-MM-dd}", employee.BirthDate),
                                       string.Format("{0:dd-MM-yyyy}", employee.BirthDate),
                                       string.Format("{0:MM-dd-yyyy}", employee.BirthDate),
                                       string.Format("{0:MMMM}", employee.BirthDate),
                                       employee.FileId,
                                       employee.BankAccount,
                                       employee.HealthInsurance,
                                       employee.Platform,
                                       employee.CurrentPosition,
                                       employee.CurrentProyect,
                                       employee.Comments,
                                       employee.EnglishLevel,
                                       employee.Notes,
                                       employee.Seniority,
                                       employee.Title,
                                       employee.University
                                   }
                               };
            Indexes.Add(x => x.ByTerm, FieldIndexing.Analyzed);
		}
    }
}
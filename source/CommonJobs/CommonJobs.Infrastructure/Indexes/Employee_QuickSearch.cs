using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System.Globalization;
using CommonJobs.Domain;

namespace CommonJobs.Infrastructure.Indexes
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
                                       string.Format("{0:yyyy-MM-dd}", employee.HiringDate),
                                       string.Format("{0:dd-MM-yyyy}", employee.HiringDate),
                                       string.Format("{0:MM-dd-yyyy}", employee.HiringDate),
                                       string.Format("{0:MMMM}", employee.HiringDate),
                                       string.Format("{0:yyyy-MM-dd}", employee.BirthDate),
                                       string.Format("{0:dd-MM-yyyy}", employee.BirthDate),
                                       string.Format("{0:MM-dd-yyyy}", employee.BirthDate),
                                       string.Format("{0:MMMM}", employee.BirthDate),
                                       employee.FileId,
                                       employee.BankAccount,
                                       employee.HealthInsurance,
                                       employee.Platform,
                                       employee.CurrentPosition,
                                       employee.CurrentProject,
                                       employee.Comments,
                                       employee.EnglishLevel,
                                       employee.Notes,
                                       employee.Seniority,
                                       employee.Degree,
                                       employee.College
                                   }
                               };
            Indexes.Add(x => x.ByTerm, FieldIndexing.Analyzed);
		}
    }
}
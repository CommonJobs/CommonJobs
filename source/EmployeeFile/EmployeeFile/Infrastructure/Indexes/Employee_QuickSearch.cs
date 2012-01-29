using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EmployeeFile.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace EmployeeFile.Infrastructure.Indexes
{
    public class Employee_QuickSearch : AbstractIndexCreationTask<Employee>
    {
        public Employee_QuickSearch()
		{
            Map = employees => from employee in employees
                               select new 
                               {
                                   Id = employee.Id,
                                   FirstName = employee.FirstName,
                                   LastName = employee.LastName,
                                   WorkingSince = employee.WorkingSince,
                                   InitialRemuneration = employee.InitialRemuneration,
                                   CurrentRemuneration = employee.CurrentRemuneration,
                                   Schedule = employee.Schedule,
                                   FileId = employee.FileId,
                                   Lunch = employee.Lunch,
                                   BankAccount = employee.BankAccount,
                                   HealthInsurance = employee.HealthInsurance,
                                   Platform = employee.Platform,
                                   CurrentPosition = employee.CurrentPosition,
                                   SalaryIncreases = employee.SalaryIncreases,
                                   CurrentProyect = employee.CurrentProyect,
                                   Agreement = employee.Agreement,
                                   Comments = employee.Comments
                               };
		}
    }
}
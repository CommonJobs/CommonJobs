using CommonJobs.Domain;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EmployeeSearching
{
    public class EmployeeByUserName_Search : AbstractIndexCreationTask<Employee>
    {
        public EmployeeByUserName_Search()
        {
            Map = m => m.Select(e => new { e.UserName });
        }
    }
}

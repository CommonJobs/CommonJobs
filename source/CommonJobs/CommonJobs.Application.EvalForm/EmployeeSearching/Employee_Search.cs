using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System.Globalization;
using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;

namespace CommonJobs.Application.Evaluations.EmployeeSearching
{
    public class Employee_Search : AbstractMultiMapIndexCreationTask<Employee_Search.Projection>
    {
        public class EmployeeEvaluationPeriod
        {
            public string Period { get; set; }

            public string Responsible { get; set; }
        }

        public class Projection
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string CurrentPosition { get; set; }
            public string Seniority { get; set; }
            public string Project { get; set; }
            public bool IsActive { get; set; }
            public EmployeeEvaluationPeriod[] EvaluationPeriods { get; set; }
        }

        public Employee_Search()
		{
            AddMap<Employee>(employees =>
                from employee in employees
                select new
                {
                    Id = employee.Id,
                    UserName = employee.UserName,
                    IsActive = employee.TerminationDate == null,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    CurrentPosition = employee.CurrentPosition,
                    Seniority = employee.Seniority,
                    Project = (string)null,
                    EvaluationPeriods = new dynamic[0]
                });


            AddMap<EmployeeEvaluation>(evaluations =>
                from evaluation in evaluations
                select new
                {
                    Id = (string)null,
                    UserName = evaluation.UserName,
                    IsActive = false,
                    FirstName = (string)null,
                    LastName = (string)null,
                    CurrentPosition = (string)null,
                    Seniority = (string)null,
                    Project = (string)null,
                    EvaluationPeriods = new[] { new { Period = evaluation.Period, Responsible = evaluation.Responsible }}
                });

            Reduce = docs =>
                from doc in docs
                group doc by doc.UserName into g
                select new
                {
                    Id = g.Where(x => x.Id != null).Select(x => x.Id).FirstOrDefault(),
                    UserName = g.Key,
                    IsActive = g.Any(x => x.IsActive),
                    FirstName = g.Where(x => x.FirstName != null).Select(x => x.FirstName).FirstOrDefault(),
                    LastName = g.Where(x => x.LastName != null).Select(x => x.LastName).FirstOrDefault(),
                    CurrentPosition = g.Where(x => x.CurrentPosition != null).Select(x => x.CurrentPosition).FirstOrDefault(),
                    Seniority = g.Where(x => x.Seniority != null).Select(x => x.Seniority).FirstOrDefault(),
                    Project = g.Where(x => x.Project != null).Select(x => x.Project).FirstOrDefault(),
                    EvaluationPeriods = g.SelectMany(x => x.EvaluationPeriods).Where(x => x != null).ToArray(),
                };
		}
    }
}

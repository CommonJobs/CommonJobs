using CommonJobs.Domain.Evaluations;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.EmployeeSearching
{
    public class EmployeeToEvaluate_Search : AbstractMultiMapIndexCreationTask<EmployeeToEvaluate_Search.Projection>
    {
        public class Evaluator
        {
            public bool IsResponsible { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
        }

        public class Projection
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string CurrentPosition { get; set; }
            public string Seniority { get; set; }
            public string Period { get; set; }
            public Evaluator[] Evaluators { get; set; }
            public bool AutoEvaluationDone { get; set; }
            public bool ResponsibleEvaluationDone { get; set; }
            public bool CompanyEvaluationDone { get; set; }
            public bool OpenToDevolution { get; set; }
            public bool Finished { get; set; }
        }

        public EmployeeToEvaluate_Search()
        {
            AddMap<EmployeeEvaluation>(evaluations =>
                from evaluation in evaluations
                select new
                {
                    Id = evaluation.Id,
                    UserName = evaluation.UserName,
                    FullName = evaluation.FullName,
                    CurrentPosition = evaluation.CurrentPosition,
                    Seniority = evaluation.Seniority,
                    Period = evaluation.Period,
                    Evaluators = new dynamic[0],
                    AutoEvaluationDone = false,
                    ResponsibleEvaluationDone = false,
                    CompanyEvaluationDone = false,
                    OpenToDevolution = false,
                    Finished = false
                });

            AddMap<EvaluationCalification>(califications =>
                from calification in califications
                select new
                {
                    Id = calification.EvaluationId,
                    UserName = calification.EvaluatedEmployee,
                    FullName = (string)null,
                    CurrentPosition = (string)null,
                    Seniority = (string)null,
                    Period = (string)null,

                    Evaluators = new dynamic[0],
                    AutoEvaluationDone = false,
                    ResponsibleEvaluationDone = false,
                    CompanyEvaluationDone = false,
                    OpenToDevolution = false,
                    Finished = false
                });

            //AddMap<Employee>(employees =>
            //    from employee in employees
            //    select new
            //    {
            //        Id = employee.Id,
            //        UserName = employee.UserName,
            //        IsActive = employee.TerminationDate == null,
            //        FirstName = employee.FirstName,
            //        LastName = employee.LastName,
            //        CurrentPosition = employee.CurrentPosition,
            //        Seniority = employee.Seniority,
            //        Project = (string)null,
            //        EvaluationPeriods = new dynamic[0]
            //    });


            //AddMap<EmployeeEvaluation>(evaluations =>
            //    from evaluation in evaluations
            //    select new
            //    {
            //        Id = (string)null,
            //        UserName = evaluation.UserName,
            //        IsActive = false,
            //        FirstName = (string)null,
            //        LastName = (string)null,
            //        CurrentPosition = (string)null,
            //        Seniority = (string)null,
            //        Project = (string)null,
            //        EvaluationPeriods = new[] { new { Period = evaluation.Period, Responsible = evaluation.Responsible } }
            //    });

            //Reduce = docs =>
            //    from doc in docs
            //    group doc by doc.UserName into g
            //    select new
            //    {
            //        Id = g.Where(x => x.Id != null).Select(x => x.Id).FirstOrDefault(),
            //        UserName = g.Key,
            //        IsActive = g.Any(x => x.IsActive),
            //        FirstName = g.Where(x => x.FirstName != null).Select(x => x.FirstName).FirstOrDefault(),
            //        LastName = g.Where(x => x.LastName != null).Select(x => x.LastName).FirstOrDefault(),
            //        CurrentPosition = g.Where(x => x.CurrentPosition != null).Select(x => x.CurrentPosition).FirstOrDefault(),
            //        Seniority = g.Where(x => x.Seniority != null).Select(x => x.Seniority).FirstOrDefault(),
            //        Project = g.Where(x => x.Project != null).Select(x => x.Project).FirstOrDefault(),
            //        EvaluationPeriods = g.SelectMany(x => x.EvaluationPeriods).Where(x => x != null).ToArray(),
            //    };
        }
    }
}

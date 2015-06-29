using CommonJobs.Domain;
using CommonJobs.Domain.Evaluations;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.EmployeeSearching
{
    public class EmployeeToEvaluate_Search : AbstractMultiMapIndexCreationTask<EmployeeToEvaluate_Search.Projection>
    {

        public class Projection
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string CurrentPosition { get; set; }
            public string CurrentSeniority { get; set; }
            public string Period { get; set; }
            public string TemplateId { get; set; }
            public string[] Evaluators { get; set; }
            public string ResponsibleId { get; set; }
            public bool AutoEvaluationDone { get; set; }
            public bool ResponsibleEvaluationDone { get; set; }
            public bool CompanyEvaluationDone { get; set; }
            public bool OpenToDevolution { get; set; }
            public bool Finished { get; set; }
        }

        public EmployeeToEvaluate_Search()
        {
            AddMap<Employee>(employees =>
                from employee in employees
                select new
                {
                    Id = (string)null,
                    UserName = employee.UserName,
                    FullName = (string)null,
                    CurrentPosition = employee.CurrentPosition,
                    Seniority = employee.Seniority,
                    Period = (string)null,
                    TemplateId = (string)null,
                    Evaluators = new dynamic[0],
                    ResponsibleId = (string)null,
                    AutoEvaluationDone = false,
                    ResponsibleEvaluationDone = false,
                    CompanyEvaluationDone = false,
                    OpenToDevolution = false,
                    Finished = false

                });

            AddMap<EmployeeEvaluation>(evaluations =>
                from evaluation in evaluations
                select new
                {
                    Id = evaluation.Id,
                    UserName = evaluation.UserName,
                    FullName = evaluation.FullName,
                    CurrentPosition = (string)null,
                    Seniority = (string)null,
                    Period = evaluation.Period,
                    TemplateId = evaluation.TemplateId,
                    Evaluators = new dynamic[0],
                    ResponsibleId = evaluation.ResponsibleId,
                    AutoEvaluationDone = false,
                    ResponsibleEvaluationDone = false,
                    CompanyEvaluationDone = false,
                    OpenToDevolution = evaluation.OpenToDevolution,
                    Finished = evaluation.Finished
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
                    TemplateId = (string)null,
                    Evaluators = (calification.Owner != CalificationType.Auto && calification.Owner != CalificationType.Company && calification.Owner != CalificationType.Responsible)
                        ? new [] { calification.EvaluatorEmployee }
                        : new dynamic[0],
                    ResponsibleId = (string)null,
                    AutoEvaluationDone = calification.Owner == CalificationType.Auto && calification.Finished,
                    ResponsibleEvaluationDone = calification.Owner == CalificationType.Responsible && calification.Finished,
                    CompanyEvaluationDone = calification.Owner == CalificationType.Company && calification.Finished,
                    OpenToDevolution = false,
                    Finished = false
                });

            Reduce = docs =>
                from doc in docs
                group doc by doc.UserName into g
                select new
                {
                    Id = g.Where(x => x.Id != null).Select(x => x.Id).FirstOrDefault(),
                    UserName = g.Key,
                    FullName = g.Where(x => x.FullName != null).Select(x => x.FullName).FirstOrDefault(),
                    CurrentPosition = g.Where(x => x.CurrentPosition != null).Select(x => x.CurrentPosition).FirstOrDefault(),
                    Seniority = g.Where(x => x.CurrentSeniority != null).Select(x => x.CurrentSeniority).FirstOrDefault(),
                    Period = g.Where(x => x.Period != null).Select(x => x.Period).FirstOrDefault(),
                    TemplateId = g.Where(x => x.TemplateId != null).Select(x => x.TemplateId).FirstOrDefault(),
                    Evaluators = g.SelectMany(x => x.Evaluators).Where(x => x != null).ToArray(),
                    ResponsibleId = g.Where(x => x.ResponsibleId != null).Select(x => x.ResponsibleId).FirstOrDefault(),
                    AutoEvaluationDone = g.Any(x => x.AutoEvaluationDone),
                    ResponsibleEvaluationDone = g.Any(x => x.ResponsibleEvaluationDone),
                    CompanyEvaluationDone = g.Any(x => x.CompanyEvaluationDone),
                    OpenToDevolution = g.Any(x => x.OpenToDevolution),
                    Finished = g.Any(x => x.Finished)
                };

            Indexes.Add(x => x.Evaluators, FieldIndexing.Analyzed);
        }
    }
}

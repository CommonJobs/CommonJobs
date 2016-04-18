using CommonJobs.Domain.Evaluations;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.EmployeeSearching
{
    class EvaluationToDelete_Search : AbstractMultiMapIndexCreationTask
    {
        public EvaluationToDelete_Search()
        {
            AddMap<EmployeeEvaluation>(evaluations =>
                from evaluation in evaluations
                select new
                {
                    Id = evaluation.Id,
                    UserName = evaluation.UserName,
                    Period = evaluation.Period
                });
            AddMap<EvaluationCalification>(califications =>
            from calification in califications
            select new
            {
                Id = calification.Id,
                UserName = calification.EvaluatedEmployee,
                Period = calification.Period
            });
        }
    }
}

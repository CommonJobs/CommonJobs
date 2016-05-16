using CommonJobs.Domain.Evaluations;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Indexes
{
    public class Period_Search : AbstractMultiMapIndexCreationTask<Period_Search.Projection>
    {
        public class Projection
        {
            public string UserName { get; set; }
            public string Period { get; set; }
        }

        public Period_Search()
        {
            AddMap<EvaluationCalification>(califications =>
                from calification in califications
                select new
                {
                    Period = calification.Period,
                    UserName = calification.EvaluatedEmployee,
                });

            AddMap<EvaluationCalification>(califications =>
               from calification in califications
               select new
               {
                   Period = calification.Period,
                   UserName = calification.EvaluatorEmployee,
               });

            Reduce = docs =>
                from doc in docs
                group doc by new { doc.UserName, doc.Period } into g
                select new
                {
                    UserName = g.Key.UserName,
                    Period = g.Key.Period
                };
        }
    }
}

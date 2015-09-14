using CommonJobs.Domain;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.Suggest
{
    public class UserName_Suggestions : AbstractIndexCreationTask<Employee, UserName_Suggestions.Projection>
    {
        public class Projection
        {
            public string UserName { get; set; }
        }

        public UserName_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    UserName = entity.UserName ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.UserName into g
                             select new
                             {
                                 UserName = g.Select(x => x.UserName.Trim()).FirstOrDefault()
                             };

            Index(x => x.UserName, FieldIndexing.Analyzed);
            Suggestion(x => x.UserName);
        }
    }
}

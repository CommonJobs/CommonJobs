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
    public class Project_Suggestions : AbstractIndexCreationTask<Employee, Project_Suggestions.Projection>
    {
        public class Projection
        {
            public string Project { get; set; }
        }

        public Project_Suggestions()
        {
            Map = employees =>
                from entity in employees
                select new
                {
                    Project = entity.CurrentProject ?? string.Empty,
                };

            Reduce = docs => from doc in docs
                             group doc by doc.Project into g
                             select new
                             {
                                 Project = g.Select(x => x.Project.Trim()).FirstOrDefault()
                             };

            Index(x => x.Project, FieldIndexing.Analyzed);
            Suggestion(x => x.Project);
        }
    }
}

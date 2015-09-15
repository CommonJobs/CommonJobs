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
    public class Skill_Suggestions : AbstractMultiMapIndexCreationTask<Skill_Suggestions.Projection>
    {
        public class Projection
        {
            public string Skill { get; set; }
        }

        public Skill_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                from skill in entity.Skills.Split(new[] { '-', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                select new
                {
                    Skill = skill
                });

            AddMap<Applicant>(applicant =>
                from entity in applicant
                from skill in entity.Skills.Split(new[] { '-', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                select new
                {
                    Skill = skill
                });

            Reduce = docs => from doc in docs
                             group doc by doc.Skill into g
                             select new
                             {
                                 Skill = g.Select(x => x.Skill.Trim()).FirstOrDefault()
                             };

            Index(x => x.Skill, FieldIndexing.Analyzed);
            Suggestion(x => x.Skill);
        }
    }
}

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
    public class TechnicalSkill_Suggestions : AbstractMultiMapIndexCreationTask<TechnicalSkill_Suggestions.Projection>
    {
        public class Projection
        {
            public string TechnicalSkillName { get; set; }
        }

        public TechnicalSkill_Suggestions()
        {
            AddMap<Employee>(employees =>
                from entity in employees
                from technicalSkillName in entity.TechnicalSkills.Select(x => x.Name)
                select new
                {
                    TechnicalSkillName = technicalSkillName
                });

            AddMap<Applicant>(applicant =>
                from entity in applicant
                from technicalSkillName in entity.TechnicalSkills.Select(x => x.Name)
                select new
                {
                    TechnicalSkillName = technicalSkillName
                });

            Reduce = docs => from doc in docs
                             group doc by doc.TechnicalSkillName into g
                             select new
                             {
                                 TechnicalSkillName = g.Select(x => x.TechnicalSkillName.Trim()).FirstOrDefault()
                             };

            Index(x => x.TechnicalSkillName, FieldIndexing.Analyzed);
            Suggestion(x => x.TechnicalSkillName);
        }
    }
}

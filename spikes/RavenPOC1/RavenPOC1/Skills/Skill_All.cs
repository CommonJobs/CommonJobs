using System.Linq;
using Raven.Client.Indexes;
using RavenPOC1.Domain;

namespace RavenPOC1.Skills
{
    internal class Skills_All : AbstractMultiMapIndexCreationTask<SkillResult>
    {
        public Skills_All()
        {
            AddMap<WorkerSearch>(searches =>  
                from search in searches
                from skill in search.Skills
                select new
                {
                    Name = skill,
                    SearchCount = 1
                });

            AddMap<Applicant>(applicants =>
                from applicant in applicants
                from skill in applicant.Skills
                select new
                {
                    Name = skill,
                    SearchCount = 0 //los skills de los aplicantes no nos interesan para asignar peso
                });

            Reduce = results => from result in results
                                group result by result.Name into skill
                                select new
                                {
                                    SearchCount = skill.Sum(x => x.SearchCount),
                                    Name = skill.Key,
                                };
        }
    }
}
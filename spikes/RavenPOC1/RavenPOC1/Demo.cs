using System;
using System.Linq;
using System.Collections.Generic;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;
using RavenPOC1.Domain;
using RavenPOC1.Skills;

namespace RavenPOC1
{
    internal class Demo
    {
        private readonly IDocumentStore _documentStore;

        public Demo(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void CreateData1()
        {
            using (var session = _documentStore.OpenSession())
            {
                var search1 = new WorkerSearch()
                {
                    Date = new DateTime(2011, 10, 5),
                    Title = "Programador Javascript",
                    Description = "Se necesita un programador de javascript que conozca de arriba a abajo jQuery.",
                    Skills = new List<string> { "Javascript", "jQuery", "C#" }
                };

                session.Store(search1);

                session.Store(new WorkerSearch()
                {
                    Date = new DateTime(2011, 11, 15),
                    Title = "Experto en COBOL",
                    Description = "Se necesita un programador de COBOL.",
                    Skills = new List<string> { "COBOL", "OTRONOMBRE1" }
                });

                session.Store(new WorkerSearch()
                {
                    Date = new DateTime(2011, 11, 20),
                    Title = "Programador FORTRAN",
                    Description = "Se necesita alguien que conozca al menos algo de FORTRAN 77.",
                    Skills = new List<string> { "FORTRAN", "OTRONOMBRE2" }
                });

                session.Store(new WorkerSearch()
                {
                    Date = new DateTime(2011, 11, 20),
                    Title = "Che Pibe",
                    Description = "Se necesita alquien que ayude con cualquier cosa, no es necesario que sepa programar"
                });

                var advert1 = new Advertisement()
                                  {
                                      Content =
                                          "¿Sos un experto en jQuery y te gustaría trabajar en una empresa divertida, dinámica y con el mejor sueldo? Envianos tu curriculum!",
                                      Start = new DateTime(2011, 10, 20),
                                      End = new DateTime(2011, 11, 10),
                                      MediaName = "Diario La Capital",
                                      WorkerSearchId = search1.Id
                                  };
                
                session.Store(advert1);

                var applicant1 = new Applicant()
                {
                    Name = "Juan Perez",
                    Country = "Argentina",
                    City = "Mar del Plata",
                    Address = "Calle Falsa 123",
                    BirthDate = new DateTime(1978, 1, 1),
                    MaritalStatus = MaritalStatus.Single,
                    Phones = new List<string> { "477-7777", "155-555555" },
                    Skills = new List<string> { "Javascript", "jQuery", "HTML", "CSS", "PHP", "json" },
                    AdvertisementIds = new List<string>()
                    {
                        advert1.Id
                    }
                };

                session.Store(applicant1);
                session.SaveChanges();
            }
        }


        internal void Run()
        {
            IndexCreation.CreateIndexes(this.GetType().Assembly, _documentStore);
            SkillsInputAutocomplete();
            SearchOfWorkerSearcs();
            GetsAnApplicantWithAllRelatedDataInOneRequest();
        }

        private void GetsAnApplicantWithAllRelatedDataInOneRequest()
        {
            using (var session = _documentStore.OpenSession())
            {
                var app = session.Query<Applicant>()
                    .Include(x => x.AdvertisementIds)
                    //.Customize(x => x.Include("AdvertisementIds")) Is the same
                    //.Customize(x => x.Include<Advertisement>(y => y.WorkerSearchId))
                    //.Customize(x => x.Include("WorkerSearchId")) Is the same
                    .Where(x => x.Name == "Juan Perez")
                    .First();

                //var ads = session.Include("WorkerSearchId").Load<Advertisement>(app.AdvertisementIds.First()); //raise a request because the include
                var ads = session.Load<Advertisement>(app.AdvertisementIds.First()); //do not raise a request
                
                var srch = session.Load<WorkerSearch> (ads.WorkerSearchId); //if ads have raised a query it does not

                //No way... TWO QUERIES
                //the first query is ok:
                //  http://localhost:8080/indexes/dynamic/Applicants?query=Name%253A%2522Juan%2520Perez%2522&start=0&pageSize=1&aggregation=None&include=AdvertisementIds&include=WorkerSearchId
                //but the server only return Advertisements
            }
        }

        private void SearchOfWorkerSearcs()
        {
            using (var session = _documentStore.OpenSession())
            {
                var a = session.Query<WorkerSearch_Search.ReduceResult, WorkerSearch_Search>().Where(x => x.Query.StartsWith("programa")).As<WorkerSearch>().ToList(); //Generates 'Query:programa*'
                var b = session.Query<WorkerSearch_Search.ReduceResult, WorkerSearch_Search>().Where(x => x.Query.Contains("programa")).As<WorkerSearch>().ToList(); //RavenDB bug: generates 'Query:programa' in place of 'Query:*programa*'
                var c = session.Query<WorkerSearch_Search.ReduceResult, WorkerSearch_Search>().Where(x => x.Query == "programador javascript").As<WorkerSearch>().ToList(); //Generates 'Query:"programador javascript"' 
                var d = session.Query<WorkerSearch_Search.ReduceResult, WorkerSearch_Search>().Where(x => x.Query == "OTRONOMBRE1").As<WorkerSearch>().ToList();
                var e = session.Query<WorkerSearch_Search.ReduceResult, WorkerSearch_Search>().Where(x => x.Query.StartsWith("OTRONOMBRE*")).As<WorkerSearch>().ToList();
                var f = session.Query<WorkerSearch>().Where(x => x.Description.Contains("programa")).ToList(); //Generates 'Query:programa*'
                var g = session.Query<WorkerSearch>().Where(x => x.Description.StartsWith("programa")).ToList(); //Generates 'Query:programa*'
                var h = session.Advanced.LuceneQuery<WorkerSearch>().Where("Description:*programa*").ToList();
            }
        }

        private void SkillsInputAutocomplete()
        {
            using (var session = _documentStore.OpenSession())
            {
                //Este no aparece en los tags
                var search2 = new WorkerSearch()
                                  {
                                      Date = new DateTime(2011, 10, 5),
                                      Title = "Programador .NET",
                                      Description = "Se necesita un programador .NET",
                                      Skills = new List<string> {"C#", "VisualBasic"}
                                  };
                session.Store(search2);


                //Me voy a traer la lista de skills ordenados por cantidad de busquedas realizadas y luego alfabeticamente para hacer un autocompleta
                var qry = session.Query<SkillResult, Skills_All>()
                    .Customize(x => x.WaitForNonStaleResults())
                    .OrderByDescending(x => x.SearchCount)
                    .ThenBy(x => x.Name)
                    .Select(x => x.Name);

                var allSkills = qry.Take(5).ToList();

                Console.WriteLine("\n\nTodos los skills: ");
                foreach (var skill in allSkills)
                {
                    Console.WriteLine(skill);
                }

                //bug (o no soportado) en raven db:
                //var skillsQueEmpiezanConJ = qry.Where(x => x.StartsWith("j")).Take(5).ToList();
                //o
                //var skillsQueEmpiezanConJ = session.Query<SkillResult, Skills_All>()
                //    .Customize(x => x.WaitForNonStaleResults())
                //    .OrderByDescending(x => x.SearchCount)
                //    .ThenBy(x => x.Name)
                //    .Select(x => x.Name)
                //    .Where(x => x.StartsWith("j"))
                //    .Take(5)
                //    .ToList();


                var skillsQueEmpiezanConJ = session.Query<SkillResult, Skills_All>()
                    .Customize(x => x.WaitForNonStaleResults())
                    .Where(x => x.Name.StartsWith("j"))
                    .OrderByDescending(x => x.SearchCount)
                    .ThenBy(x => x.Name)
                    .Select(x => x.Name)
                    .Take(5)
                    .ToList();

                Console.WriteLine("\n\nSkillsQueEmpiezanConJ: ");
                foreach (var skill in skillsQueEmpiezanConJ)
                {
                    Console.WriteLine(skill);
                }
            }
        }
    }
}
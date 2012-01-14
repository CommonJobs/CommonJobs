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
                                         AdvertisementResponses = new List<AdvertisementResponse>()
                                        {
                                            new AdvertisementResponse()
                                            {
                                                AdvertisementId = advert1.Id,
                                                AdvertisementMediaName = advert1.MediaName,
                                            }
                                        }
                                     };

                session.Store(applicant1);
                session.SaveChanges();
            }
        }


        internal void Run()
        {
            IndexCreation.CreateIndexes(typeof(Skills_All).Assembly, _documentStore);
            CreateData1();
            SkillsInputAutocomplete();
            SearchOfWorkerSearcs();
            GetsAnApplicantWithAllRelatedDataInOneRequest();
        }

        private void GetsAnApplicantWithAllRelatedDataInOneRequest()
        {
            using (var session = _documentStore.OpenSession())
            {
                //Console.ReadLine();
                //var ad = session.Query<Advertisement>().Include(x => x.WorkerSearchId).First();
                //var ad = session.Query<Advertisement>().First();
                var adid = "";
                var ad = session.Include("WorkerSearchId").Load<Advertisement>(adid);
                Console.WriteLine("ad");
                //Console.ReadLine();
                var search = session.Load<WorkerSearch>(ad.WorkerSearchId);
                Console.WriteLine("search");
                //Console.ReadLine();

                //TODO: it generates an temporal index, I should generate a permanent index before.
                //session.Query<Applicant>().Where(x => x.Name == "Juan Perez").Include(x => x.AdvertisementResponses)

                //var advertisments = session.Query<AdvertisementResponse>().Include(x => x.ApplicantId).ToArray();
                //var applicants = session.Load<Applicant>(advertisments.Select(x => x.))
                //Console.ReadLine();
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
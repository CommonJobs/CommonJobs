using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Embedded;
using Raven.Database.Server;
using Raven.Client.Document;

namespace RavenPOC1
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpEndpointRegistration.RegisterHttpEndpointTarget();
            //using (var documentStore = CreateEmbeddaleDocumentStore().Initialize())
            using (var documentStore = CreateRealDocumentStore().Initialize())
            {
                //documentStore.DatabaseCommands.
                //documentStore.DisableAggressiveCaching();
                var demo = new Demo(documentStore);
                demo.CreateData1();
                demo.Run();
                Console.ReadLine();
            }
        }

        static DocumentStore CreateRealDocumentStore()
        {
            return new DocumentStore()
            {
                Url = "http://localhost:8080"
            };
        }

        static DocumentStore CreateEmbeddaleDocumentStore()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
            return new EmbeddableDocumentStore
            {
                RunInMemory = true,
                //DataDirectory = "Data",
                UseEmbeddedHttpServer = true,
            };
        }
    }
}

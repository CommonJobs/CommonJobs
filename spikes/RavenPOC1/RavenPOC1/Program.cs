using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace RavenPOC1
{
    class Program
    {
        static void Main(string[] args)
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);
            HttpEndpointRegistration.RegisterHttpEndpointTarget();
            using (var documentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true,
                //DataDirectory = "Data",
                UseEmbeddedHttpServer = true
            }.Initialize())
            {
                var demo = new Demo(documentStore);
                demo.Run();
            }
        }
    }
}

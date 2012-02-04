using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace CommonJobs.Mvc
{
    public abstract class CommonJobsApplication : HttpApplication
    {
        public CommonJobsApplication()
        {
            EndRequest += (sender, args) => RavenSessionManager.CloseCurrentSession();
        }

        protected void InitializeDocumentStore(Assembly[] indexAssemblies, string connectionStringName = "RavenDB", string errorUrl = "~/RavenNotReachable.htm")
        {
            RavenSessionManager.InitializeDocumentStore(indexAssemblies, connectionStringName, errorUrl);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using CommonJobs.Utilities;

namespace CommonJobs.Raven.Mvc
{
    public abstract class CommonJobsApplication : HttpApplication
    {
        static bool initialized = false;
        static readonly object block = new object();

        public CommonJobsApplication()
        {
            EndRequest += (sender, args) => RavenSessionManager.CloseCurrentSession();
            lock (block)
            {
                if (!initialized)
                    Initialize();
            }
        }

        protected void Initialize()
        {
            initialized = true;
            InitializeAppVersion();
            CommonJobsBindingConfiguration();
            InitializeDocumentStore();
        }

        /// <summary>
        /// Reads entry point assembly version.
        /// </summary>
        private void InitializeAppVersion()
        {
            AppName = this.GetType().BaseType.Assembly.GetName();
            AppNameHash = Encoding.IntToBase64urlEncoding(AppName.FullName.GetHashCode());
        }

        protected abstract Assembly[] GetIndexAssemblies();

        public static AssemblyName AppName { get; private set; }
        public static string AppNameHash { get; private set; }

        protected virtual string GetConnectionStringName()
        {
            return "RavenDB";
        }

        protected virtual string GetConnectionErrorUrl()
        {
            return "~/RavenNotReachable.htm";
        }

        private void InitializeDocumentStore()
        {
            RavenSessionManager.InitializeDocumentStore(GetIndexAssemblies(), GetConnectionStringName(), GetConnectionErrorUrl());
        }

        private void CommonJobsBindingConfiguration()
        {
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonDotNetValueProviderFactory());
            ModelBinders.Binders.DefaultBinder = new JsonDotNetModelBinder();
        }
    }
}
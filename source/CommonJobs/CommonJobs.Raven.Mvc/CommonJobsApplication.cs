using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using CommonJobs.Utilities;
using Raven.Client.Document;
using Raven.Client.Listeners;
using NLog;

namespace CommonJobs.Raven.Mvc
{
    public abstract class CommonJobsApplication : HttpApplication
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        static bool initialized = false;
        static readonly object block = new object();

        public CommonJobsApplication()
        {
            EndRequest += (sender, args) =>
            {
                var error = HttpContext.Current.Server.GetLastError();
                if (error != null)
                {
                    log.ErrorException("Uncatched exception", error);
                    LogManager.Flush(1000);
                }               
                RavenSessionManager.CloseCurrentSession(error != null);
            };
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
        protected virtual IEnumerable<IDocumentConversionListener> GetConversionListeners() { yield break; }
        protected virtual IEnumerable<IDocumentDeleteListener> GetDeleteListeners() { yield break; }
        protected virtual IEnumerable<IDocumentQueryListener> GetQueryListeners() { yield break; }
        protected virtual IEnumerable<IDocumentStoreListener> GetDocumentStoreListeners() { yield break; }

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
            global::Raven.Client.MvcIntegration.RavenProfiler.InitializeFor(RavenSessionManager.DocumentStore);
            RegisterListeners();
        }

        private void RegisterListeners()
        {
            var documentStore = (DocumentStore)RavenSessionManager.DocumentStore;
            foreach (var listener in GetConversionListeners())
            {
                documentStore.RegisterListener(listener);
            }
            foreach (var listener in GetDeleteListeners())
            {
                documentStore.RegisterListener(listener);
            }
            foreach (var listener in GetQueryListeners())
            {
                documentStore.RegisterListener(listener);
            }
            foreach (var listener in GetDocumentStoreListeners())
            {
                documentStore.RegisterListener(listener);
            }
        }

        private void CommonJobsBindingConfiguration()
        {
            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonDotNetValueProviderFactory());
            ModelBinders.Binders.DefaultBinder = new JsonDotNetModelBinder();
        }
    }
}
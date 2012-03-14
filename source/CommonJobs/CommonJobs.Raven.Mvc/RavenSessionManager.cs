using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using System.Reflection;

namespace CommonJobs.Raven.Mvc
{
    public static class RavenSessionManager
    {
        private const string CURRENT_REQUEST_RAVEN_SESSION_KEY = "_COMMOMJOBS_CURRENT_REQUEST_RAVEN_SESSION_";
       
        public static IDocumentStore DocumentStore { get; private set; }

        internal static IDocumentSession GetCurrentSession()
        {
            var session = HttpContext.Current.Items[CURRENT_REQUEST_RAVEN_SESSION_KEY] as IDocumentSession;
            if (session == null)
            {
                session = DocumentStore.OpenSession();
                HttpContext.Current.Items[CURRENT_REQUEST_RAVEN_SESSION_KEY] = session;
            }
            return session;
        }

        internal static void CloseCurrentSession()
        {
            using (var session = (IDocumentSession)HttpContext.Current.Items[CURRENT_REQUEST_RAVEN_SESSION_KEY])
            {
                if (session == null)
                    return;

                if (HttpContext.Current.Server.GetLastError() != null)
                    return;

                session.SaveChanges();
            }
        }

        internal static void InitializeDocumentStore(Assembly[] indexAssemblies, string connectionStringName, string errorUrl)
        {
            if (DocumentStore != null)
                return; // prevent misuse

            DocumentStore = new DocumentStore() { ConnectionStringName = connectionStringName }.Initialize();

            TryCreatingIndexesOrRedirectToErrorPage(indexAssemblies, errorUrl);
        }

        private static void TryCreatingIndexesOrRedirectToErrorPage(Assembly[] indexAssemblies, string errorUrl)
        {
            try
            {
                foreach (var assembly in indexAssemblies)
                    IndexCreation.CreateIndexes(assembly, DocumentStore);
            }
            catch (WebException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null)
                    throw;

                switch (socketException.SocketErrorCode)
                {
                    case SocketError.AddressNotAvailable:
                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                    case SocketError.TimedOut:
                    case SocketError.ConnectionRefused:
                    case SocketError.HostDown:
                    case SocketError.HostUnreachable:
                    case SocketError.HostNotFound:
                        HttpContext.Current.Response.Redirect(errorUrl);
                        break;
                    default:
                        throw;
                }
            }
        }
    }
}
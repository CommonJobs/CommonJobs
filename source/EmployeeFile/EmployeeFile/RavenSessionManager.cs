using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using EmployeeFile.Infrastructure.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace EmployeeFile
{
    public static class RavenSessionManager
    {
        private const string CURRENT_REQUEST_RAVEN_SESSION_KEY = "CurrentRequestRavenSession";
        
        public static IDocumentStore DocumentStore { get; private set; }

        public static IDocumentSession GetCurrentSession()
        {
            var session = HttpContext.Current.Items[CURRENT_REQUEST_RAVEN_SESSION_KEY] as IDocumentSession;
            if (session == null)
            {
                session = DocumentStore.OpenSession();
                HttpContext.Current.Items[CURRENT_REQUEST_RAVEN_SESSION_KEY] = session;
            }
            return session;
        }

        public static void CloseCurrentSession()
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

        public static void InitializeDocumentStore()
        {
            if (DocumentStore != null)
                return; // prevent misuse

            DocumentStore = new DocumentStore
                                {
                                    ConnectionStringName = "CommonJobsDB"
                                }.Initialize();

            TryCreatingIndexesOrRedirectToErrorPage();
        }

        private static void TryCreatingIndexesOrRedirectToErrorPage()
        {
            try
            {
                IndexCreation.CreateIndexes(typeof(NullIndex).Assembly, DocumentStore);
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
                        HttpContext.Current.Response.Redirect("~/RavenNotReachable.htm");
                        break;
                    default:
                        throw;
                }
            }
        }
    }
}
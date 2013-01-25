using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Application.SharedLinks;
using CommonJobs.Infrastructure.Mvc;
using CommonJobs.Infrastructure.Mvc.Authorize;
using System.Configuration;
using CommonJobs.Utilities;

namespace CommonJobs.Mvc.UI.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class DocumentationAttribute : Attribute
    {
        public static string BaseUrl 
        { 
            get { return ConfigurationManager.AppSettings["CommonJobs/DocumentationBaseUrl"].AppendIfDoesNotEndWith("/"); }
        }

        public string DocumentPath  { get; set; }

        public string GetUrl()
        {
            return string.Format("{0}{1}", BaseUrl, DocumentPath);
        }
        
        public DocumentationAttribute(string documentPath)
        {
            DocumentPath = documentPath;
        }
    }
}
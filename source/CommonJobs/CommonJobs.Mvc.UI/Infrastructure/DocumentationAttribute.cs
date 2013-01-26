using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonJobs.Mvc.UI.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class DocumentationAttribute : Attribute
    {
        public string DocumentPath  { get; set; }

        public DocumentationAttribute(string documentPath)
        {
            DocumentPath = documentPath;
        }
    }
}
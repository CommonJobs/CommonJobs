using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc
{
    internal abstract class ScriptManagerEntry
    {
        public int Priority { get; set; }
        public int SetOrder { get; set; }
    }

    internal class CssReferenceEntry : ScriptManagerEntry
    {
        public string Path { get; set; }
    }

    internal class JsReferenceEntry : ScriptManagerEntry
    {
        public string Path { get; set; }
    }

    internal class GlobalJavascriptEntry : ScriptManagerEntry
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
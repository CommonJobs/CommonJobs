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
        public Object HtmlAttributes { get; set; }
    }

    internal abstract class ReferenceEntry : ScriptManagerEntry
    {
        public string Path { get; set; }
        public abstract string TagName();
        public abstract string ReferenceAttributeName();
        public abstract object DefaultAttributes();
        public abstract bool SelfClosed();
        public bool OmitAppVersion { get; set; }
    }

    internal class CssReferenceEntry : ReferenceEntry
    {
        public override string TagName() { return "link"; }
        public override string ReferenceAttributeName() { return "href"; }
        public override object DefaultAttributes() { return new { rel = "stylesheet", type = "text/css" }; }
        public override bool SelfClosed() { return true; }
    }

    internal class JsReferenceEntry : ReferenceEntry
    {
        public override string TagName() { return "script"; }
        public override string ReferenceAttributeName() { return "src"; }
        public override object DefaultAttributes() { return new { type = "text/javascript" }; }
        public override bool SelfClosed() { return false; }
    }

    internal class GlobalJavascriptEntry : ScriptManagerEntry
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Hosting;

namespace CommonJobs.JavaScript
{
    public class IronJavascriptScriptEngine : IJavascriptScriptEngine
    {
        private readonly CSharp.Context engine = new CSharp.Context();

        public object Run(string script)
        {
            return engine.Execute(script);
        }
    }
}

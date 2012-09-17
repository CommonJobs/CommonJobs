using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jurassic;

namespace CommonJobs.JavaScript
{
    public class JurassicJavascriptScriptEngine : IJavascriptScriptEngine
    {
        private readonly ScriptEngine engine = new ScriptEngine();

        public object Run(string script)
        {
            return engine.Evaluate(script);
        }
    }
}

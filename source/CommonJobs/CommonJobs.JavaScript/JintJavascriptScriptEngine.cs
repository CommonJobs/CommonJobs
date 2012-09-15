using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;

namespace CommonJobs.JavaScript
{
    public class JintJavascriptScriptEngine : IJavascriptScriptEngine
    {
        private readonly JintEngine engine = new JintEngine();

        public object Run(string script)
        {
            return engine.Run(script);
        }
    }
}

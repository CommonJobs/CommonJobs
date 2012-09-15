using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.JavaScript
{
    public interface IJavascriptScriptEngine
    {
        object Run(string script);
    }
}

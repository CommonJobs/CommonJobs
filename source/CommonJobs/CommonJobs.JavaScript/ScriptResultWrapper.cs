using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.JavaScript
{
    public abstract class ScriptResultWrapper
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public class ScriptResultWrapper<TResult> : ScriptResultWrapper
    {
        public TResult Result { get; set; }
    }
}

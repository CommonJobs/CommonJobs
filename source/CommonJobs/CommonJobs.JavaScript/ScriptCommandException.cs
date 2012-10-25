using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.JavaScript
{
    public class ScriptCommandException : Exception
    {
        public ScriptResultWrapper ResultWrapper { get; private set; }
        public string Details { get { return ResultWrapper.Details;  } }

        public ScriptCommandException(ScriptResultWrapper resultWrapper)
            : base(string.IsNullOrWhiteSpace(resultWrapper.Details) ? resultWrapper.Message : resultWrapper.Message + " (" + resultWrapper.Details + ")")
        {
            ResultWrapper = resultWrapper;
        }
    }
}

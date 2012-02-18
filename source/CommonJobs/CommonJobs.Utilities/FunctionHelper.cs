using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Utilities
{
    public static class FunctionHelper
    {
        /// <summary>
        /// It runs the functions in the provided order while the result is null. When the result is not null, it is returned.
        /// </summary>
        public static TResult FirtsNotNull<TResult>(params Func<TResult>[] funcs) where TResult : class
        {
            TResult r = null;
            foreach (var f in funcs)
            {
                r = f();
                if (r != null)
                    break;
            }
            return r;
        }
    }
}

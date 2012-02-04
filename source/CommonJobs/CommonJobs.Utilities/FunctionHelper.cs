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
        public static TResult FirtsNotNull<T, TResult>(T param, params Func<T, TResult>[] funcs) where TResult : class
        {
            TResult r = null;
            foreach (var f in funcs)
            {
                r = f(param);
                if (r != null)
                    break;
            }
            return r;
        }
    }
}

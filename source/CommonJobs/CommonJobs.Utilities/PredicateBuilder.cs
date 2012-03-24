using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace CommonJobs.Utilities
{
    /// <summary>
    /// http://www.albahari.com/nutshell/predicatebuilder.aspx
    /// http://www.mindscapehq.com/forums/thread/3232
    /// </summary>
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, expr2.Body), 
                expr1.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(expr1.Body, expr2.Body), 
                expr1.Parameters);
        }
    }
}
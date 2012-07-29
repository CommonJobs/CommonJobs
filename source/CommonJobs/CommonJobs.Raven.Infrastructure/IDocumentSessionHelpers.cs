using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace CommonJobs.Raven.Infrastructure
{
    public static  class IDocumentSessionHelpers
    {
        //It is not necessary, but it is an example
        //public static T Load<T>(this IDocumentSession session, long numericIdentityPart)
        //{
        //    var conventions = session.Advanced.DocumentStore.Conventions;
        //    var typeTagName = conventions.GetTypeTagName(typeof(T));
        //    var tag = conventions.TransformTypeTagNameToDocumentKeyPrefix(typeTagName);
        //    return session.Load<T>(tag + conventions.IdentityPartsSeparator + numericIdentityPart);
        //}

        public static long? ExtractNumericIdentityPart(this IDocumentSession session, object entity)
        {
            var id = session.Advanced.GetDocumentId(entity);
            if (id != null)
            {
                var conventions = session.Advanced.DocumentStore.Conventions;
                var lastPart = id.Split(new[] { conventions.IdentityPartsSeparator }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                long value;
                if (lastPart != null && long.TryParse(lastPart, out value))
                {
                    return value;
                }
            }
            return null;
        }
    }
}

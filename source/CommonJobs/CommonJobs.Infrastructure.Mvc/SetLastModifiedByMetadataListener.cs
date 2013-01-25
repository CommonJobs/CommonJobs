using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using Raven.Client.Listeners;
using Raven.Json.Linq;


namespace CommonJobs.Infrastructure.Mvc
{
    public class SetLastModifiedByMetadataListener : IDocumentStoreListener
    {
        public const string UpdatedByMetadataKey = "Last-Modified-By";

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
        }

        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.Name != null)
            {
                var parts = HttpContext.Current.User.Identity.Name.Split(new[] { '\\' });
                metadata[UpdatedByMetadataKey] = parts.LastOrDefault();
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonJobs.Domain;
using CommonJobs.Infrastructure.SharedLinks;
using CommonJobs.Raven.Mvc;
using CommonJobs.Raven.Mvc.Authorize;

namespace CommonJobs.Mvc.UI.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SharedEntityAlternativeAuthorizationAttribute : Attribute, IAlternativeAuthorizationAttribute
    {
        public string EntityIdKey { get; set; }
        public string SharedCodeKey { get; set; }
        
        public SharedEntityAlternativeAuthorizationAttribute()
            : this("id", "sharedCode")
        {
        }

        public SharedEntityAlternativeAuthorizationAttribute(string entityIdKey, string sharedCodeKey)
        {
            EntityIdKey = entityIdKey;
            SharedCodeKey = sharedCodeKey;
        }
        
        private void ReadEntityIdAndSharedCode(AuthorizationContext filterContext, out string entityId, out string sharedCode)
        {
            var valueProvider = filterContext.Controller.ValueProvider;
            /*
             * Otra opcion es no utilizar filterContext.Controller.ValueProvider para no inicializarlo, pero eso no es garantía de que nunca se hubiera inicializado.
             * 
             *   var valueProvider = ValueProviderFactories.Factories.GetValueProvider(filterContext.Controller.ControllerContext);
             */

            var entityIdResult = valueProvider.GetValue(EntityIdKey);
            entityId = entityIdResult == null ? null : entityIdResult.AttemptedValue;

            var sharedCodeResult = valueProvider.GetValue(SharedCodeKey);
            sharedCode = sharedCodeResult == null ? null : sharedCodeResult.AttemptedValue;
        }
        
        public bool Authorize(AuthorizationContext filterContext)
        {
            var controller = filterContext.Controller as CommonJobsController;
            if (controller == null)
                return false;

            string entityId;
            string sharedCode;
            ReadEntityIdAndSharedCode(filterContext, out entityId, out sharedCode);

            if (sharedCode == null)
                return false;

            var entityIdFound = controller.Query(new SearchSharedEntity(sharedCode, entityId));

            if (entityIdFound == null)
                return false;

            filterContext.Controller.ValueProvider = new OverrideValueProvider(filterContext.Controller.ValueProvider, 
                new Dictionary<string, string>() { { EntityIdKey, SharedCodeKey }, { entityIdFound, sharedCode } });

            return true;
        }
    }
}
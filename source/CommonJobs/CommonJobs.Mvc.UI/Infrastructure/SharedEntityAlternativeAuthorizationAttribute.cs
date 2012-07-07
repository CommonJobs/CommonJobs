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

            filterContext.Controller.ValueProvider = new OverrideValueProvider(filterContext.Controller.ValueProvider, EntityIdKey, SharedCodeKey, entityIdFound, sharedCode);
            /*
             * La otra opción es setear a mano los valores en RouteData, pero tengo el problema de la precendencia ya que hay otras fuentes antes de route
             * Además si ya se habia inicializado filterContext.Controller.ValueProvider no me acepta las modificaciones
             *
             *    filterContext.RouteData.Values[EntityIdKey] = indexResult.SharedEntityId;
             *    filterContext.RouteData.Values[SharedCodeKey] = indexResult.SharedLink.SharedCode;
             */

            return true;
        }

        private class OverrideValueProvider : IValueProvider
        {
            IValueProvider OriginalValueProvider { get; set; }
            Dictionary<string, ValueProviderResult> HardcodedValues { get; set; }

            public OverrideValueProvider(IValueProvider originalValueProvider, string entityIdkey, string sharedCodeKey, string entityId, string sharedCode)
            {
                OriginalValueProvider = originalValueProvider;
                HardcodedValues = new Dictionary<string, ValueProviderResult>()
                {
                    { entityIdkey, new ValueProviderResult(entityId, entityId, System.Globalization.CultureInfo.InvariantCulture) },
                    { sharedCodeKey, new ValueProviderResult(sharedCode, sharedCode, System.Globalization.CultureInfo.InvariantCulture) }
                };
            }

            public bool ContainsPrefix(string prefix)
            {
                return HardcodedValues.ContainsKey(prefix) || OriginalValueProvider.ContainsPrefix(prefix);
            }

            public ValueProviderResult GetValue(string key)
            {
                if (HardcodedValues.ContainsKey(key))
                    return HardcodedValues[key];
                else
                    return OriginalValueProvider.GetValue(key);
            }
        }
    }
}
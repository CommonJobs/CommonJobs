using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CommonJobs.Infrastructure.Mvc
{
    /// <summary>
    /// Esta clase permite sobreescribir el value provider de un controlador para permitir inyectar valores arbitrarios
    /// </summary>
    /// <remarks>
    /// La otra opción es setear a mano los valores en RouteData, pero tengo el problema de la precendencia ya que hay otras fuentes antes de route
    /// Además si ya se habia inicializado filterContext.Controller.ValueProvider no me acepta las modificaciones
    /// filterContext.RouteData.Values[EntityIdKey] = indexResult.SharedEntityId;
    /// filterContext.RouteData.Values[SharedCodeKey] = indexResult.SharedLink.SharedCode;
    /// </remarks>
    public class OverrideValueProvider : IValueProvider
    {
        IValueProvider OriginalValueProvider { get; set; }
        Dictionary<string, ValueProviderResult> HardcodedValues { get; set; }

        public OverrideValueProvider(IValueProvider originalValueProvider, IDictionary<string, string> values)
        {
            OriginalValueProvider = originalValueProvider;
            HardcodedValues = values.ToDictionary(
                x => x.Key,
                x => new ValueProviderResult(x.Value, x.Value, System.Globalization.CultureInfo.InvariantCulture));
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

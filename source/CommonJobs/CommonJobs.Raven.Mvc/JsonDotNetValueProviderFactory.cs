using System.Dynamic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;

namespace CommonJobs.Raven.Mvc
{
    public sealed class JsonDotNetValueProviderFactory : ValueProviderFactory
    {
        private static void AddToBackingStore(Dictionary<string, object> backingStore, string prefix, JToken value)
        {
            if (value is JObject)
            {
                var jObject = value as JObject;
                foreach (JProperty jProperty in jObject.Properties())
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, jProperty.Name), jProperty.Value);
                }
            }
            else if (value is JArray)
            {
                var jArray = value as JArray;
                for (int i = 0; i < jArray.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), jArray[i]);
                }
            }
            else if (value is JValue)
            {
                var jValue = value as JValue;
                if (jValue.Value == null)
                    backingStore[prefix] = null;
                else
                    backingStore[prefix] = jValue.Value.ToString(); //ToString added by Andres in order to allow Int64 to Decimal conversions
            }
            else
            {
                throw new Exception(string.Format("JToken is of unsupported type {0}", value.GetType()));
            }
        }

        private static JObject GetDeserializedJson(ControllerContext controllerContext)
        {
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // not JSON request
                return null;
            }

            var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            string bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText))
            {
                // no JSON data
                return null;
            }

            //return JObject.Parse(bodyText);
            return JsonConvert.DeserializeObject(bodyText, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }) as JObject;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            JObject jsonData = GetDeserializedJson(controllerContext);
            if (jsonData == null)
            {
                return null;
            }

            var backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            AddToBackingStore(backingStore, String.Empty, jsonData);
            return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }
    }
}
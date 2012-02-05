using System.Dynamic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    public class JsonDotNetModelBinder : DefaultModelBinder
    {
        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //{
            //if (bindingContext.ModelName == "InitialSalary" || bindingContext.ModelName == "Happenings[0].Salary" || bindingContext.ModelName == "Happenings[0].RealDate")
            //{
                //ConvertProviderResult(bindingContext.ModelState, bindingContext.ModelName, bindingContext.UnvalidatedValueProvider.GetValue(bindingContext.ModelName), bindingContext.ModelType);
                //A first chance exception of type 'System.InvalidOperationException' occurred in System.Web.Mvc.dll
                //base {System.SystemException}: {"The parameter conversion from type 'System.Int64' to type 'System.Decimal' failed because no type converter can convert between these types."}
            //}
        //    return base.BindModel(controllerContext, bindingContext);
        //}

        public bool PersonalizedBehaviorApplicable(ModelBindingContext bindingContext, out Type realType)
        {
            //TODO: remove restrictions ¿?
            if (bindingContext.ModelMetadata.IsComplexType && !bindingContext.ModelMetadata.ModelType.IsArray && !bindingContext.ModelMetadata.ModelType.IsGenericType)
            {
                var realTypeValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".$type");
                if (realTypeValue != null)
                {
                    var modelType = bindingContext.ModelType;
                    realType = Type.GetType(realTypeValue.RawValue.ToString());
                    if (modelType != realType && modelType.IsAssignableFrom(realType))
                        return true;
                }
            }
            realType = null;
            return false;
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            Type realType;
            if (PersonalizedBehaviorApplicable(bindingContext, out realType))
                return base.CreateModel(controllerContext, bindingContext, realType);

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        protected override ComponentModel.PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type realType;
            if (PersonalizedBehaviorApplicable(bindingContext, out realType))
            {
                ModelBindingContext newBindingContext = new ModelBindingContext()
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => bindingContext.Model, realType),
                    ModelName = bindingContext.ModelName,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                return base.GetModelProperties(controllerContext, newBindingContext);
            }
            return base.GetModelProperties(controllerContext, bindingContext);
        }
    }

    public sealed class JsonDotNetValueProviderFactory : ValueProviderFactory
    {
        //public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        //{
        //    if (controllerContext == null)
        //        throw new ArgumentNullException("controllerContext");

        //    if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
        //        return null;

        //    var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
        //    var bodyText = reader.ReadToEnd();
        //    var a = JsonConvert.DeserializeObject<CommonJobs.Domain.Employee>(bodyText);
        //    var b = JsonConvert.DeserializeObject(bodyText, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }) as JObject;
        //    var expando = JsonConvert.DeserializeObject<ExpandoObject>(bodyText, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, Converters = new List<JsonConverter> { new ExpandoObjectConverter() } });
        //    return String.IsNullOrEmpty(bodyText) ? null : new DictionaryValueProvider<object>(expando, CultureInfo.CurrentCulture);
        //}
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
                    backingStore[prefix] = jValue.Value.ToString();
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
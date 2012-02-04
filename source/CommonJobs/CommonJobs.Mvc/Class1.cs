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
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.BindModel(controllerContext, bindingContext);
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            //I am working here
        //internal void BindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext, object model) {
        //    // need to replace the property filter + model object and create an inner binding context
        //    ModelBindingContext newBindingContext = CreateComplexElementalModelBindingContext(controllerContext, bindingContext, model);

        //    // validation
        //    if (OnModelUpdating(controllerContext, newBindingContext)) {
        //        BindProperties(controllerContext, newBindingContext);
        //        OnModelUpdated(controllerContext, newBindingContext);
        //    }
        //}
            if (bindingContext.ModelMetadata.IsComplexType && !bindingContext.ModelMetadata.ModelType.IsArray)
            {
                var realType = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".$type");
                if (realType != null)
                {
                    var type = Type.GetType(realType.RawValue.ToString());
                    if (modelType.IsAssignableFrom(type))
                        return base.CreateModel(controllerContext, bindingContext, type);
                }
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        protected override ComponentModel.PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.GetModelProperties(controllerContext, bindingContext);
        }

        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }

        protected override ComponentModel.ICustomTypeDescriptor GetTypeDescriptor(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //it does not work
            //if (bindingContext.ModelMetadata.IsComplexType && !bindingContext.ModelMetadata.ModelType.IsArray)
            //{
            //    var realType = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + ".$type");
            //    if (realType != null)
            //    {
            //        var type = Type.GetType(realType.RawValue.ToString());
            //        if (bindingContext.ModelType.IsAssignableFrom(type))
            //            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
            //    }
            //}

            return base.GetTypeDescriptor(controllerContext, bindingContext);
        }

        protected override void OnModelUpdated(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.OnModelUpdated(controllerContext, bindingContext);
        }

        protected override bool OnModelUpdating(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return base.OnModelUpdating(controllerContext, bindingContext);
        }

        protected override void OnPropertyValidated(ControllerContext controllerContext, ModelBindingContext bindingContext, ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            base.OnPropertyValidated(controllerContext, bindingContext, propertyDescriptor, value);
        }

        protected override bool OnPropertyValidating(ControllerContext controllerContext, ModelBindingContext bindingContext, ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            return base.OnPropertyValidating(controllerContext, bindingContext, propertyDescriptor, value);
        }

        protected override void SetProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
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
                backingStore[prefix] = jValue.Value;
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
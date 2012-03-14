using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CommonJobs.Raven.Mvc
{
    /// <summary>
    /// This ModelBinder try to read $type property of Newtonsoft Json.Net serialized objects.
    /// </summary>
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

        protected override System.ComponentModel.PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
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
}

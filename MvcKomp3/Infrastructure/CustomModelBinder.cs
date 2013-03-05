using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKomp3.Infrastructure
{
    public abstract class CustomModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentException("Controller Context");
            if(bindingContext == null)
                throw new ArgumentException("Binding Context");


            return CreateModel(controllerContext, bindingContext);
        }

        public abstract object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext);

        private ValueProviderResult GetValueProviderResult(ControllerContext controllerContext, ModelBindingContext bindingContext, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            string bindingModelNameWithKey = String.Format("{0}.{1}", bindingContext.ModelName, key);
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingModelNameWithKey);
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix)
                valueResult = bindingContext.ValueProvider.GetValue(key);
            if (valueResult == null)
                return null;

            return valueResult;
        }
        
        protected T GetValueClass<T>(ControllerContext controllerContext, ModelBindingContext bindingContext, string key)
            where T : class
        {
            ValueProviderResult valueResult = GetValueProviderResult(controllerContext, bindingContext, key);
            if (valueResult == null)
                return null;

            bindingContext.ModelState.SetModelValue(key, valueResult);

            try
            {
                return (T)valueResult.ConvertTo(typeof(T));
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }

        protected Nullable<T> GetValueStruct<T>(ControllerContext controllerContext, ModelBindingContext bindingContext, string key)
            where T : struct
        {
            ValueProviderResult valueResult = GetValueProviderResult(controllerContext, bindingContext, key);
            if (valueResult == null)
                return null;

            bindingContext.ModelState.SetModelValue(key, valueResult);

            try
            {
                return new Nullable<T>((T)valueResult.ConvertTo(typeof(T)));
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return null;
            }
        }
    }
}
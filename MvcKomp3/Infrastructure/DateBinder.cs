using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MvcKomp3.Infrastructure
{
    public class DateBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result == null)
                return null;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result);

            DateTime date;
            if (DateTime.TryParse(result.AttemptedValue, out date))
                return date;

            int resInt;
            if (int.TryParse(result.AttemptedValue, out resInt))
                return new DateTime(resInt, 1, 1);

            bindingContext.ModelState.AddModelError("eer", new Exception());

            var year = bindingContext.ValueProvider.GetValue("Year");
            var y2 = controllerContext.HttpContext.Request.Form.Get("Year");
            return DateTime.Now;
        }

        private T Get<T>(ControllerContext controllerCtx, ModelBindingContext bindingCtx, string name)
        {
            string fullName = name;
            if (!string.IsNullOrWhiteSpace(bindingCtx.ModelName))
                fullName = bindingCtx.ModelName + "." + name;
            ValueProviderResult result = bindingCtx.ValueProvider.GetValue(fullName);

            ModelState modelState = new ModelState { Value = result };
            bindingCtx.ModelState.Add(fullName, modelState);
            
            ModelMetadata metadata = bindingCtx.PropertyMetadata[name];
            string attempteValue = result.AttemptedValue;
            if (metadata.ConvertEmptyStringToNull && string.IsNullOrWhiteSpace(attempteValue))
                attempteValue = null;

            T model;
            bool invalid = false;
            try
            {
                model = (T)result.ConvertTo(typeof(T));
                metadata.Model = model;
            }
            catch (Exception)
            {
                model = default(T);
                metadata.Model = attempteValue;
                invalid = true;
            }

            var errors = from m in ModelValidatorProviders.Providers.GetValidators(metadata, controllerCtx)
                    from v in m.Validate(bindingCtx.Model)
                    select v.Message;
            foreach (var error in errors)
                modelState.Errors.Add(error);
            if (invalid && modelState.Errors.Count == 0)
                modelState.Errors.Add(String.Format("The value {0} is not a Valid value for {1}", attempteValue, metadata.GetDisplayName()));

            return model;
        }

    }

    public class DateDefaultBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {

            if (propertyDescriptor.Name == "StartDate")
            {
                var date = controllerContext.HttpContext.Request.Form["StartDate"];

                DateTime dt = DateTime.Parse(date);
                    
                //int.Parse(controllerContext.HttpContext.Request.Form["month"]),
                //int.Parse(controllerContext.HttpContext.Request.Form["day"]));

                propertyDescriptor.SetValue(bindingContext.Model, dt.AddYears(5));

                return;
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);

            //ValueProviderResult result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            //base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}
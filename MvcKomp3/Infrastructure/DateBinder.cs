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
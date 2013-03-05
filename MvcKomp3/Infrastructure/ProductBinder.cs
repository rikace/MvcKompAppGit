using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKomp3.Infrastructure;
using MvcKompApp.ViewModels;

namespace MvcKompMvc4.Infrastructure
{
    public class ProductBinder : CustomModelBinder
    {
        public override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var r1 = GetValueStruct<double>(controllerContext, bindingContext, "Price");// bindingContext.ValueProvider.GetValue("Price");
            var val = r1.Value;

            var r2 = GetValueClass<string>(controllerContext, bindingContext, "Color"); // bindingContext.ValueProvider.GetValue("Color");
            var val2 = r2;

         

            bindingContext.ModelState.AddModelError("Price",
                            "Please put a value more then $10");

            return new ProductViewModel() { Color = r2, Price = r1.Value, Name = "Bugghina" };
//            return new ProductViewModel() { Color = r2.AttemptedValue, Price = double.Parse(r1.AttemptedValue), Name = "Bugghina" };
        }
    }
    //public class ProductBinder : IModelBinder
    //{
    //    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        //return bindingContext.ModelName;

    //        var r1 = bindingContext.ValueProvider.GetValue("Price");
    //        var val = r1.RawValue;

    //        var r2 = bindingContext.ValueProvider.GetValue("Color");
    //        var val2 = r2.RawValue;

    //        //var name = bindingContext.ValueProvider.GetValue("Name");
    //        //var isNull = name.AttemptedValue == null;
    //        //var result1 = Get<double>(controllerContext, bindingContext, "Price");
    //        //var result2 = Get<string>(controllerContext, bindingContext, "Color");
    //        //var result3 = Get<string>(controllerContext, bindingContext, "Name");

    //        bindingContext.ModelState.AddModelError("Price",
    //                        "Please put a value more then $10");

    //        return new ProductViewModel() { Color = r2.AttemptedValue, Price = double.Parse(r1.AttemptedValue), Name="Bugghina" };
    //    }

    //    private T Get<T>(ControllerContext controllerCtx, ModelBindingContext bindingCtx, string name)
    //    {
    //        string fullName = name;
    //        if (!string.IsNullOrWhiteSpace(bindingCtx.ModelName))
    //            fullName = bindingCtx.ModelName + "." + name;
    //        ValueProviderResult result = bindingCtx.ValueProvider.GetValue(fullName);

    //        ModelMetadata metadata = bindingCtx.PropertyMetadata[name];
    //        if (result == null || result.AttemptedValue == null)
    //            return default(T);
    //        string attempteValue = result.AttemptedValue;
    //        if (metadata.ConvertEmptyStringToNull && string.IsNullOrWhiteSpace(attempteValue))
    //            attempteValue = null;

    //        T model;

    //        model = (T)result.ConvertTo(typeof(T));
    //        return model;
    //    }

    //}
}

/*
    public class ValidatingModelBinder : DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext,
            ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor,
            object value)
        {

            // make sure we call the base implementation
            base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);

            // perform our property-level validation
            switch (propertyDescriptor.Name)
            {
                case "ClientName":
                    if (string.IsNullOrEmpty((string)value))
                    {
                        bindingContext.ModelState.AddModelError("ClientName",
                            "Please enter your name");
                    }
                    break;
                case "Date":
                    if (bindingContext.ModelState.IsValidField("Date") &&
                        DateTime.Now > ((DateTime)value))
                    {
                        bindingContext.ModelState.AddModelError("Date",
                            "Please enter a date in the future");
                    }
                    break;
                case "TermsAccepted":
                    if (!((bool)value))
                    {
                        bindingContext.ModelState.AddModelError("TermsAccepted",
                            "You must accept the terms");
                    }
                    break;
            }
        }

        protected override void OnModelUpdated(ControllerContext controllerContext,
            ModelBindingContext bindingContext)
        {

            // make sure we call the base implementation
            base.OnModelUpdated(controllerContext, bindingContext);

            // get the model
            Appointment model = bindingContext.Model as Appointment;

            // apply our model-level validation
            if (model != null &&
                bindingContext.ModelState.IsValidField("ClientName") &&
                bindingContext.ModelState.IsValidField("Date") &&
                model.ClientName == "Joe" &&
                model.Date.DayOfWeek == DayOfWeek.Monday)
            {
                bindingContext.ModelState.AddModelError("",
                    "Joe cannot book appointments on Mondays");
            }
        }
    }
*/
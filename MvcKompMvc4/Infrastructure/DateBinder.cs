using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MvcKomp3.Infrastructure
{
    public class DateBinder:IModelBinder
    {
        
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var year = bindingContext.ValueProvider.GetValue("Year");
            var y2 = controllerContext.HttpContext.Request.Form.Get("Year");
            return DateTime.Now;
        }
    }
}
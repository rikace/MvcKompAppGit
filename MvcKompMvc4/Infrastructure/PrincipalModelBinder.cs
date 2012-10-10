using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace MvcKomp3.Infrastructure
{
    /* Incoming HTTP request carriers data.
     * The catch is that the data is embedded into POST form values, and possibly the URL itself.  
     * Enter the DefaultModelBinder, wchich can magically convert from values and route data into objects.
     * To implement a custom ModelBinder you need to implement the IMdoelBinder interface.
      */
    public class PrincipalModelBinder:IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentException("ControllerContext");

            if (bindingContext == null)
                throw new ArgumentException("BindingContext");

            IPrincipal principal = controllerContext.HttpContext.User;

            return principal;

        }
    }
}
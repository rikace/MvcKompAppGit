using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MvcKomp3.Filters
{
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override Boolean IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request.IsAjaxRequest();
        }
    }

    /*
     * [AjaxOnly]
        public ActionResult Edit([Bind(Prefix = "customerList")] String customerId)
        {
            var model = _service.EditCustomer(customerId);
 
            // Add a custom response header to easily pass extra information to the 
            // client Ajax engine--alternative would be packing everything into JSON and NOT 
            // using the MVC Ajax infrastructure. Name of the header is arbitrary.
            if (Request.IsAjaxRequest())
                HttpContext.Response.AddHeader("Content-Title", model.Customer.CompanyName);
            return PartialView("_customerEditor", model);
        }
*/

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKomp3.Filters
{
    public class AjaxActionOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest() &&
                !filterContext.IsChildAction
            )
            {
                filterContext.Result = new HttpNotFoundResult();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKompMvc4.Infrastructure
{
    public class IsAjaxAttribute: ActionMethodSelectorAttribute
    {
         private readonly bool _ajax;
         public IsAjaxAttribute(bool ajax)
         {
             _ajax = ajax;
         }
         public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
         {
             return _ajax == controllerContext.HttpContext.Request.IsAjaxRequest();
         }
     }
}
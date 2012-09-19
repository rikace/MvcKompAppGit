using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace MVCControlsToolkit.ActionFilters
{
    public class AcceptViewHintAttribute : ActionFilterAttribute
    {
        private JsonRequestBehavior jsBehavior;
        public AcceptViewHintAttribute(JsonRequestBehavior jsBehavior = JsonRequestBehavior.DenyGet)
        {
            this.jsBehavior = jsBehavior;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string hint = filterContext.RequestContext.HttpContext.Request.Params["ViewHint"];
            if (hint == null) hint = filterContext.RequestContext.RouteData.Values["ViewHint"] as string;
            if (!string.IsNullOrWhiteSpace(hint) && hint.Length<=100 && new Regex(@"^\w+$").IsMatch(hint) )
            {
                
               
                    ViewResultBase res = filterContext.Result as ViewResultBase;
                    if (res != null)
                    {
                        if (hint == "json")
                        {
                            JsonResult jr = new JsonResult();
                            jr.Data = res.ViewData.Model;
                            jr.JsonRequestBehavior = jsBehavior;
                            filterContext.Result = jr;
                        }
                        else
                        {
                            res.ViewName = hint;
                        }
                    }
                
            }
            base.OnActionExecuted(filterContext);
        }
    }
}

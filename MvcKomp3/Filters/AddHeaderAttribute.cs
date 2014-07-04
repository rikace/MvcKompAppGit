using System;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class AddHeaderAttribute : ActionFilterAttribute
    {
        public String Name { get; set; }
        public String Value { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
                return;

            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(Value))
                filterContext.RequestContext.HttpContext.Response.AddHeader(Name, Value);
            return;
        }
    }
}
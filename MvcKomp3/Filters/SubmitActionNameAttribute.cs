using System;
using System.Reflection;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class SubmitActionNameAttribute : ActionNameSelectorAttribute
    {
        public String SubmitButton { get; set; }
        public String ActionName { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, String actionName, MethodInfo methodInfo)
        {
            if (!String.Equals(actionName, ActionName, StringComparison.OrdinalIgnoreCase))
                return false;

            var o = controllerContext.HttpContext.Request[SubmitButton];
            return o != null;
        }
    }
}
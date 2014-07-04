using System;
using System.Reflection;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class OnlyIfPostedFromButtonAttribute : ActionMethodSelectorAttribute
    {
        public String SubmitButton { get; set; }

        public override Boolean IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            // NOTE
            // An action method selector is invoked for each controller request and for each method. If multiple
            // candidates pass the selection process, you win an exception

            // Check if this request is coming through the specified submit button
            var o = controllerContext.HttpContext.Request[SubmitButton];
            return o != null;
        }
    }
}
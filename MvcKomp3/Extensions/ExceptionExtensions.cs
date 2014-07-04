using System;
using System.Web.Mvc;

namespace BookSamples.Components.Extensions
{
    public static class ExceptionExtensions
    {
        public static void SwitchToErrorView(this ExceptionContext context, String view = "error", String master = "")
        {
            var controllerName = context.RouteData.Values["controller"] as String;
            var actionName = context.RouteData.Values["action"] as String;
            var model = new HandleErrorInfo(context.Exception, controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = view,
                MasterName = master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = context.Controller.TempData
            };
            context.Result = result;

            StopPropagation(context);
        }

        private static void StopPropagation(ExceptionContext context, Boolean handled = true)
        {
            context.ExceptionHandled = handled;
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.StatusCode = 500;
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    } 
}
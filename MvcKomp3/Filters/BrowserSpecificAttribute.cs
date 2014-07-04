using System;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class BrowserSpecificAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (viewResult == null)
                return;

            // Figure out the view name
            var context = filterContext.Controller.ControllerContext;
            var viewName = viewResult.ViewName;
            if (String.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");
            if (String.IsNullOrEmpty(viewName))
                return;

            // Resolve the view selector
            var viewSelector = DependencyResolver.Current.GetService(typeof (IViewSelector)) as IViewSelector ??
                               new DefaultViewSelector();

            // Figure out the browser name
            var isMobileDevice = context.HttpContext.Request.Browser.IsMobileDevice;
            var browserName = (isMobileDevice ?"mobile" :context.HttpContext.Request.Browser.Browser);

            // Get the name of the browser-specific view to use
            var newViewName = viewSelector.GetViewName(viewName, browserName);
            if (String.IsNullOrEmpty(newViewName)) 
                return;

            // Is there such a view?
            var result = ViewEngines.Engines.FindView(context, newViewName, viewResult.MasterName);
            if (result.View != null)
            {
                viewResult.ViewName = newViewName;
            }
        }
    }
}
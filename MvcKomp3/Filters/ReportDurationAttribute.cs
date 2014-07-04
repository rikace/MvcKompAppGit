using System;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class ReportDurationAttribute : AddHeaderAttribute
    {
        protected DateTime StartTime;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            StartTime = DateTime.Now;
            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var timeSpan = DateTime.Now - StartTime;
            Value = timeSpan.Milliseconds.ToString();
            base.OnActionExecuted(filterContext);
        }
    }
}
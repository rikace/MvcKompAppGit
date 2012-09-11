using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKompApp.Filters
{
    public class UseStopwatchAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            filterContext.Controller.ViewData["stopWatch"] = stopWatch;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Stopwatch stopWatch = (Stopwatch)filterContext.Controller.ViewData["stopWatch"];
            stopWatch.Stop();
            Random r = new Random();

            filterContext.Controller.ViewData["elapsedTime"] = stopWatch.ElapsedMilliseconds
                + " milliseconds -   Rand  " + r.Next(1000).ToString();
        }

    }

}
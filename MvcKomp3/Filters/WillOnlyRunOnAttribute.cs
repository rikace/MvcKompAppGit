using System;
using System.Reflection;
using System.Web.Mvc;

namespace BookSamples.Components.Filters
{
    public class WillOnlyRunOnAttribute : ActionMethodSelectorAttribute
    {
        private readonly DayOfWeek _dayOfWeek = DayOfWeek.Sunday;
        private readonly bool _exitOnError;

        public WillOnlyRunOnAttribute(DayOfWeek day) : this(day, true)
        {
        }
        public WillOnlyRunOnAttribute(DayOfWeek day, bool exitOnError)
        {
            _dayOfWeek = day;
            _exitOnError = exitOnError;

            ErrorViewName = "Error";
            ErrorMessage = String.Format("Your type of request will only be processed on <span class='placeholderText'>{0}</span>s. Try again in a few days.",
                _dayOfWeek);
        }

        public bool ExitOnError { get; set; }
        public String ErrorViewName { get; set; }
        public String ErrorMessage { get; set; }

        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            var shouldRun = IsToday();
            if (!shouldRun)
            {
                if(_exitOnError)
                    return false;

                controllerContext.RouteData.Values["action"] = ErrorViewName;
                controllerContext.Controller.ViewData[ErrorViewName] = ErrorMessage;
            }
            return true;
        }

        private bool IsToday()
        {
            var today = DateTime.Now.DayOfWeek;
            return today == _dayOfWeek;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcNotes.Extensions.ErrorHandling;

namespace MvcNotes.Filters
{
    public class NotifyException:FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if(filterContext.IsChildAction)
                return;

            Debug.WriteLine("Error");

            if (filterContext.Exception != null)
            {
                ExceptionHelpers.SwitchToErrorView(filterContext);
                // Log Error   
            }
        }
    }
}
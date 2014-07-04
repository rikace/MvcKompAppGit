using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BookSamples.Components.Filters.Axpect
{
    public class AxpectActionInvoker : ControllerActionInvoker
    {
        protected override ActionExecutedContext InvokeActionMethodWithFilters(
            ControllerContext controllerContext,
            IList<IActionFilter> filters,
            ActionDescriptor actionDescriptor,
            IDictionary<String, Object> parameters)
        {
            // Check whether (dynamic-loading) filters are defined for this action
            var methodFilters = AxpectFramework.LoadFromConfigurationAsActionFilter(actionDescriptor);
            
            // Apply filter(s)
            foreach (var filter in methodFilters)
            {
                // Add the filter
                filters.Add(filter);
            }

            // Exit
            return base.InvokeActionMethodWithFilters(controllerContext, filters, actionDescriptor, parameters);
        }
    }
}
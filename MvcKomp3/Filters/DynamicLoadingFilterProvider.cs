using System.Collections.Generic;
using System.Web.Mvc;
using BookSamples.Components.Filters.Axpect;

namespace BookSamples.Components.Filters
{
    public class DynamicLoadingFilterProvider : IFilterProvider
    {
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return AxpectFramework.LoadFromConfigurationAsFilter(actionDescriptor);
        }
    }
}
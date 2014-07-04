using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace BookSamples.Components.Filters.Axpect
{
    public class AxpectFramework
    {
        public static IList<Filter> LoadFromConfigurationAsFilter(ActionDescriptor actionDescriptor)
        {
            var listOfFilterNames = LoadFiltersForAction(actionDescriptor);

            var filters = new List<Filter>();
            foreach (var filterName in listOfFilterNames)
            {
                // Instantiate and initialize filter (if params are specified)
                var filterInstance = GetFilterInstance(filterName);
                if (filterInstance == null)
                    continue;
                InitializeFilter(filterName, filterInstance);

                // Add to the list to return
                var filter = new Filter(filterInstance, FilterScope.Action, -1);
                filters.Add(filter);
            }

            return filters;
        }
        
        public static IList<IActionFilter> LoadFromConfigurationAsActionFilter(ActionDescriptor actionDescriptor)
        {
            var listOfFilterNames = LoadFiltersForAction(actionDescriptor);

            var filters = new List<IActionFilter>();
            foreach (var filterName in listOfFilterNames)
            {
                // Instantiate and initialize filter (if params are specified)
                var filterInstance = GetFilterInstance(filterName);
                if (filterInstance == null)
                    continue;
                InitializeFilter(filterName, filterInstance);

                // Add to the list to return
                filters.Add(filterInstance);
            }

            return filters;
        }


        #region Helpers
        private static IEnumerable<String> LoadFiltersForAction(ActionDescriptor actionDescriptor)
        {
            var methodFilters = new List<String>();
            var methodName = String.Format("{0}.{1}", actionDescriptor.ControllerDescriptor.ControllerName, actionDescriptor.ActionName);

            // Check if method name is aspected in config
            var aspectableMethodNameSetting = ConfigurationManager.AppSettings["AspectableActions"];
            if (String.IsNullOrEmpty(aspectableMethodNameSetting))
                return methodFilters;
            var aspectableMethodNames = aspectableMethodNameSetting.Split(',');
            if (aspectableMethodNames.Length == 0)
                return methodFilters;

            var methodAspectSetting = ConfigurationManager.AppSettings[methodName];
            if (String.IsNullOrEmpty(methodAspectSetting))
                return new List<String>();

            return methodAspectSetting.Split('|');
        }

        private static IActionFilter GetFilterInstance(String filter)
        {
            var filterType = Type.GetType(filter);
            if (filterType == null)
                return null;

            return Activator.CreateInstance(filterType) as IActionFilter;
        }

        private static void InitializeFilter(String filter, IActionFilter filterInstance)
        {
            var aspectParamSetting = ConfigurationManager.AppSettings[filter];
            if (!String.IsNullOrEmpty(aspectParamSetting))
            {
                var aspectParams = aspectParamSetting.Split('|');
                if (aspectParams.Length > 0)
                {
                    foreach (var aspectParam in aspectParams)
                    {
                        var tokens = aspectParam.Split('=');
                        if (tokens.Length != 2)
                            continue;

                        var paramName = tokens[0];
                        var paramValue = tokens[1];

                        // Set properties on the instance of the aspect
                        var propInfo = filterInstance.GetType().GetProperty(paramName);
                        propInfo.SetValue(filterInstance, paramValue, null);
                    }
                }
            }
        }
        #endregion
    }
}
using System;
using System.Web;
using System.Web.Routing;
using BookSamples.Components.Extensions;

namespace BookSamples.Components.Routes
{
    public class EvenConstraint : IRouteConstraint
    {
        public Boolean Match(HttpContextBase httpContext, Route route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // Check whether the parameter is an even number
            var paramValue = values[parameterName] as String;
            if (paramValue == null)
                return false;

            var id = paramValue.ToInt();
            return id % 2 == 0;
        }
    }
}
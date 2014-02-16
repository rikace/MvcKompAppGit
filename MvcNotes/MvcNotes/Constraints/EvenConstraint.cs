using System;
using System.Web;
using System.Web.Routing;

namespace MvcNotes.Extensions.Constraints
{
    public class EvenConstraint : IRouteConstraint
    {
        public Boolean Match(HttpContextBase httpContext, Route route, String parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // Check whether the parameter is an even number
            var paramValue = values[parameterName] as String;
            if (paramValue == null)
                return false;

            var id = int.Parse(paramValue);
            return id%2 == 0; 
        }
    }
}
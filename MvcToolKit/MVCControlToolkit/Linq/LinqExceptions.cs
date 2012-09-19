using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Linq
{

    public class NotAllowedExpressionException : ForbiddenException
        {
            public NotAllowedExpressionException(string name) :
                base(string.Format("SafeQuery doesn't allow {0}", name))
            {

            }
            public NotAllowedExpressionException() :
                base(string.Format("SafeQuery allows only logical operators, comparison operators, access to properties, StartsWith, EndsWith, and Contains"))
            {

            }
        }
    public class NotAllowedColumnException : ForbiddenException
        {
            public NotAllowedColumnException(string propertyName, Type type) :
                base(string.Format("Sorting and Filtering are not allowed on property {0} of {1}. Are you missing a CanSortAttribute?", propertyName, type.FullName))
            {

            }
        }
    public class NotAllowedFilteringException : ForbiddenException
        {
            public NotAllowedFilteringException(string operationName, string propertyName, Type type) :
                base(string.Format("Filtering operation {0} is not allowed on property {1} of {2}.", operationName, propertyName, type.FullName))
            {

            }
        }
    
}

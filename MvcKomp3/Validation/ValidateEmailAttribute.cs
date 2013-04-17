using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcKompApp.Validation
{
    /// <summary>
    /// Custom annotation attribute.
    /// Used to validate Email addresses.
    /// </summary>
    public class ValidateEmailAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Constructor of ValidateEmailAttribute.
        /// </summary>
        public ValidateEmailAttribute() :
            base("^([a-zA-Z0-9_\\-\\.\\+]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$")
        {
            ErrorMessage = "Invalid email address";
        }

    }
}
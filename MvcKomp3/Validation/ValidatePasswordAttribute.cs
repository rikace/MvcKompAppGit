using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MvcKompApp.Validation
{
    /// <summary>
    /// Custom annotation attribute.
    /// Used to validate Passwords. Min length is 7, max is 100.
    /// </summary>
    public class ValidatePasswordAttribute : StringLengthAttribute
    {
        /// <summary>
        /// Constructor of ValidatePasswordAttribute.
        /// Used to validate Email addresses.
        /// </summary>
        public ValidatePasswordAttribute()
            : base(100)
        {
            MinimumLength = 7;
            ErrorMessage = "Minimum password length is 7 characters";
        }
    }
}
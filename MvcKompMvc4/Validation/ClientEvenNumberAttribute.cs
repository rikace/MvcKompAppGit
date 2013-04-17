using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DataAnnotations.Components
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ClientEvenNumberAttribute : ValidationAttribute, IClientValidatable
    {
        private const String EvenErrorMessage = "The value must be even.";
        private const String MultipleOf4ErrorMessage = "The value must be a multiple of 4.";

        // Whether the number is even and also a multiple of 4
        public Boolean MultipleOf4 { get; set; }

        //protected override ValidationResult IsValid(Object value, ValidationContext context)
        //{
        //    return ValidationResult.Success;
        //}
        public override Boolean IsValid(Object value)
        {
            if (value == null)
                return false;

            var x = -1;
            try
            {
                x = (Int32)value;
            }
            catch
            {
                return false;
            }

            if (x % 2 > 0)
                return false;

            if (!MultipleOf4)
                return true;

            // Is multiple of 4?
            return (x % 4 == 0);
        }

        #region IClientValidatable
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var errorMessage = ErrorMessage;
            if (String.IsNullOrEmpty(errorMessage))
                errorMessage = (MultipleOf4 ? MultipleOf4ErrorMessage : EvenErrorMessage);

            var rule = new ModelClientValidationRule { ValidationType = "iseven", ErrorMessage = errorMessage };
            rule.ValidationParameters.Add("generic", "Just an error");
            rule.ValidationParameters.Add("multipleof4", MultipleOf4);
            yield return rule;
        }
        #endregion
    }
}

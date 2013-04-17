using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DataAnnotations.Components
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GreaterThanDateAttribute : ValidationAttribute, IClientValidatable
    {
        public GreaterThanDateAttribute(String otherPropertyName)        
            :base("{0} must be greater than {1}")
        {
            OtherPropertyName = otherPropertyName;
        }

        public String OtherPropertyName { get; set; }

        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherPropertyName); 
            var otherDate = (DateTime)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null); 
            var thisDate = (DateTime)value; 
            if (thisDate <= otherDate)
            {
                var message = "Dino"; //FormatErrorMessage(validationContext.DisplayName); 
                return new ValidationResult(message);
            }

            return null;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = "Dino"; //FormatErrorMessage(metadata.GetDisplayName());    
            rule.ValidationType = "greater"; // This is what the jQuery.Validation expects    
            rule.ValidationParameters.Add("other", OtherPropertyName); // This is the 2nd parameter    
            yield return rule;
        }
    }
}

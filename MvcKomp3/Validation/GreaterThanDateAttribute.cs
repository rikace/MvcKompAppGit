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
            : base("{0} must be greater than {1}")
        {
            OtherPropertyName = otherPropertyName;
        }

        public String OtherPropertyName { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(ErrorMessage, name, OtherPropertyName);
        }

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
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationType = "greater"; // This is what the jQuery.Validation expects    
            rule.ValidationParameters.Add("other", OtherPropertyName); // This is the 2nd parameter    
            yield return rule;
        }
        /*
         /// <reference path="jquery-1.7.1-vsdoc.js" />
         /// <reference path="jquery.validate-vsdoc.js" />
         /// <reference path="jquery.validate.unobtrusive.js" />

        jQuery.validator.addMethod("greater", function (value, element, param) {
            return Date.parse(value) > Date.parse($(param).val());
        });

        jQuery.validator.unobtrusive.adapters.add("greater", ["other"], function (options) {
            options.rules["greater"] = "#" + options.params.other;
            options.messages["greater"] = options.message;
        });        
        */

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKomp3.Validation
{
    public class DateComesLaterClientAttribute : DateComesLaterAttribute, IClientValidatable
    {
        public DateComesLaterClientAttribute(string otherDateProperty) : base(otherDateProperty) { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = GetErrorMessage(metadata.ContainerType, metadata.GetDisplayName()),
                ValidationType = "later",
            };

            rule.ValidationParameters.Add("other", "*." + OtherDateProperty);

            yield return rule;
        }
    }

    public class DateComesLaterAttribute : ValidationAttribute
    {
        public const string DefaultErrorMessage = "'{0}' must be after '{1}'";

        protected readonly string OtherDateProperty;

        public DateComesLaterAttribute(string otherDateProperty)
        {
            OtherDateProperty = otherDateProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type type = validationContext.ObjectType;

            var earlierDate = (DateTime?)type.GetProperty(OtherDateProperty).GetValue(instance, null);
            var date = (DateTime?)value;

            if (date > earlierDate)
                return ValidationResult.Success;

            string errorMessage = GetErrorMessage(validationContext.ObjectType, validationContext.DisplayName);

            return new ValidationResult(errorMessage);
        }

        protected string GetErrorMessage(Type containerType, string displayName)
        {
            ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, containerType,
                                                                                           OtherDateProperty);
            var otherDisplayName = metadata.GetDisplayName();
            return ErrorMessage ?? string.Format(DefaultErrorMessage, displayName, otherDisplayName);
        }
    }
}
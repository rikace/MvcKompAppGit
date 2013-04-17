using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcKompApp.Models
{
    public class MaxWordsValidationAttribute : ValidationAttribute, IClientValidatable
    {
        private int _maxWords;
        public MaxWordsValidationAttribute(int maxWords)
            : base("{0} has to many words")
        {
            _maxWords = maxWords;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var valueAsString = value as string;
                if (valueAsString.Split(' ').Length > _maxWords)
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }
            return ValidationResult.Success;
        }

        #region IClientValidatable Members

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            // var rule = new ModelClientValidationEqualToRule
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.DisplayName);
            rule.ValidationParameters.Add("wordcount", _maxWords);
            rule.ValidationType="maxwords";
            yield return rule;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DataAnnotations.Components
{
    public class EvenNumberValidator : DataAnnotationsModelValidator<EvenNumberAttribute>
    {
        private readonly Boolean _multipleOf4;
        private readonly String _message;
        private const String EvenErrorMessage = "The value must be even.";
        private const String MultipleOf4ErrorMessage = "The value must be a multiple of 4.";

        public EvenNumberValidator(ModelMetadata metadata, ControllerContext context, EvenNumberAttribute attribute)
            : base(metadata, context, attribute)
        {
            _multipleOf4 = attribute.MultipleOf4;
            _message = attribute.ErrorMessage;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var errorMessage = _message;
            if (String.IsNullOrEmpty(_message))
                errorMessage = (_multipleOf4 ? MultipleOf4ErrorMessage : EvenErrorMessage);

            var rule = new ModelClientValidationRule { ErrorMessage = errorMessage, ValidationType = "even" };
            rule.ValidationParameters.Add("multipleOf4", _multipleOf4);     // whether to check also if is a multiple of 4

            return new[] { rule };
        }
    }
}



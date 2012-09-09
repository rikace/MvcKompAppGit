using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcKompApp.Validation
{
    public class PriceAttribute : ValidationAttribute
    {
        public double MinPrice { get; set; }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            var price = (double)value;
            if (price < MinPrice)
            {
                return false;
            }
            double cents = price - Math.Truncate(price);
            if (cents < 0.99 || cents >= 0.995)
            {
                return false;
            }

            return true;
        }
    }

    public class PriceValidator : DataAnnotationsModelValidator<PriceAttribute>
    {
        double _minPrice;
        string _message;

        public PriceValidator(ModelMetadata metadata, ControllerContext context
          , PriceAttribute attribute)
            : base(metadata, context, attribute)
        {
            _minPrice = attribute.MinPrice;
            _message = attribute.ErrorMessage;
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = _message,
                ValidationType = "price"
            };
            rule.ValidationParameters.Add("min", _minPrice);

            return new[] { rule };
        }
    } 
}
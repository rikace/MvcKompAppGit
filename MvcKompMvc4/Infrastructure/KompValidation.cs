using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MvcKompApp.Controllers
{
    public class KompValidation:IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validateCtx)
        {
            if (true)
                yield return new ValidationResult("Error message from KompValidation");
        }

    }
}
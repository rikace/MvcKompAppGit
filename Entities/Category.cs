using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public partial class Category:IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validatiomContext)
        {
            yield return new ValidationResult("Error");
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace DataAnnotations.Components
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EvenNumberAttribute : ValidationAttribute
    {
        // Whether the number is even and also a multiple of 4
        public Boolean MultipleOf4 { get; set; }

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
    }
}

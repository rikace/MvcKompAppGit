using System;
using System.ComponentModel.DataAnnotations;

namespace MvcKompApp.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class MinLenghtAttribute:ValidationAttribute
    {
        public override bool IsValid(Object value)
        {
            string valueAsString = value as string;
            return !String.IsNullOrWhiteSpace(valueAsString) &&
                    valueAsString.Length >= 3;
        }
    }
}
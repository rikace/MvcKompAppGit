using System;

namespace BookSamples.Components.Extensions
{
    public static class StringExtensions
    {
        public static Uri AsUri(this String theString)
        {
            return new Uri(theString);
        }

        public static Int32 ToInt(this String theString, Int32 defaultValue = Int32.MinValue)
        {
            var number = defaultValue;
            var result = Int32.TryParse(theString, out number);
            return number;
        } 
    }
}
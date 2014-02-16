using System;
using System.Web.Mvc;

namespace MvcNotes.Binders
{
    public class DateModelBinder : IModelBinder
    {
        /// <summary>
        /// This will return a DateTime object from a sequence of day/month/year fields.
        /// </summary>
        /// <param name="controllerContext">Context of the controller</param>
        /// <param name="bindingContext">Binding information</param>
        /// <returns>A newly created DateTime object</returns>
        public Object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            // This will return a DateTime object
            var theDate = default(DateTime); 

            // Try to read from posted data. xxx.Day|xxx.Month|xxx.Year is assumed.
            var day = FromPostedData<int>(bindingContext, "Day");
            var month = FromPostedData<int>(bindingContext, "Month");
            var year = FromPostedData<int>(bindingContext, "Year");

            return CreateDateOrDefault(year, month, day, theDate);
        }

        // Helper routines
        private static T FromPostedData<T>(ModelBindingContext context, String id)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (String.IsNullOrEmpty(id))
                return default(T);

            // Get the value from any of the input collections
            var key = String.Format("{0}.{1}", context.ModelName, id);
            var result = context.ValueProvider.GetValue(key);
            if (result == null && context.FallbackToEmptyPrefix)
            {
                // Try without prefix 
                result = context.ValueProvider.GetValue(id);
                if (result == null)
                    return default(T);
            }

            // Set the state of the model property resulting from value
            context.ModelState.SetModelValue(id, result);

            // Return the value converted (if possible) to the target type
            T valueToReturn = default(T);
            try
            {
                valueToReturn = (T)result.ConvertTo(typeof(T));
            }
            catch(NullReferenceException e)
            {
            }
            return valueToReturn;
        }

        private static DateTime CreateDateOrDefault(Int32 year, Int32 month, Int32 day, DateTime? defaultDate)
        {
            var theDate = defaultDate ?? default(DateTime);
            try
            {
                theDate = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            return theDate;
        }
    }
}


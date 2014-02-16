using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MvcNotes.Infrastructure
{
    public static class Mapper
    {
        public static TDest Map<TSource, TDest>(TSource source)
            where TSource : class
            where TDest : class, new()
        {
            var destination = new TDest();
            foreach (
                PropertyInfo destProp in
                    typeof (TDest).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
            {
                PropertyInfo sourceProp =
                    typeof (TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault(
                            p =>
                                p.Name.ToUpper().Equals(destProp.Name.ToUpper()) &&
                                p.PropertyType == destProp.PropertyType);

                if (sourceProp != null)
                {
                    destProp.SetValue(destination, sourceProp.GetValue(source, null), null);
                }
            }
            return destination;
        }

        public static void CopyModel(object from, object to)
        {
            if (from == null || to == null)
            {
                return;
            }

            PropertyDescriptorCollection fromProperties = TypeDescriptor.GetProperties(from);
            PropertyDescriptorCollection toProperties = TypeDescriptor.GetProperties(to);

            foreach (PropertyDescriptor fromProperty in fromProperties)
            {
                PropertyDescriptor toProperty = toProperties.Find(fromProperty.Name, true /* ignoreCase */);
                if (toProperty != null && !toProperty.IsReadOnly)
                {
                    // Can from.Property reference just be assigned directly to to.Property reference?
                    bool isDirectlyAssignable = toProperty.PropertyType.IsAssignableFrom(fromProperty.PropertyType);

                    // Is from.Property just the nullable form of to.Property?
                    bool liftedValueType = (isDirectlyAssignable)
                        ? false
                        : (Nullable.GetUnderlyingType(fromProperty.PropertyType) == toProperty.PropertyType);

                    if (isDirectlyAssignable || liftedValueType)
                    {
                        object fromValue = fromProperty.GetValue(from);
                        if (isDirectlyAssignable || (fromValue != null && liftedValueType))
                        {
                            toProperty.SetValue(to, fromValue);
                        }
                    }
                }
            }
        }
    }
}

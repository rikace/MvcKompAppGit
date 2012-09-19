/* ****************************************************************************
 *
 * Copyright (c) Francesco Abbruzzese. All rights reserved.
 * francesco@dotnet-programming.com
 * http://www.dotnet-programming.com/
 * 
 * This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
 * and included in the license.txt file of this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.Exceptions;
using MVCControlsToolkit.Core;
using System.Globalization;

namespace MVCControlsToolkit.DataAnnotations
{
     
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DynamicRangeAttribute : ValidationAttribute
    {
        
        public DynamicRangeAttribute(Type targetType, string message):
            base(message) 
        {
            TargetType = targetType;
            getMinMaxForType();
        }
        public DynamicRangeAttribute(Type targetType) :
            base(AnnotationsRsources.StandardError)
        {
            TargetType = targetType;
            getMinMaxForType();
        }
        public string SMinimum
        {
            set
            {
                if(value==null)
                {
                    Minimum=null;
                    return;
                }
                IComparable res;
                try
                {
                    res = (value as IConvertible).ToType(TargetType, CultureInfo.InvariantCulture) as IComparable;
                    Minimum = res;
                }
                catch
                {
                    throw (new FormatException(AnnotationsRsources.InvalidFormat));
                }
            }
            get
            {
                return null;
            }

        }
        public string SMaximum
        {
            set
            {
                if(value==null)
                {
                    Maximum=null;
                    return;
                }
                IComparable res;
                try
                {
                    res = (value as IConvertible).ToType(TargetType, CultureInfo.InvariantCulture) as IComparable;
                    Maximum = res;
                }
                catch
                {
                    throw (new FormatException(AnnotationsRsources.InvalidFormat));
                }
            }
            get
            {
                return null;
            }

        }
        
        
        Type TargetType { get; set; }
        public IComparable Minimum{get; protected set;}
        public IComparable Maximum { get; protected set; }
       
        public string DynamicMaximum {get; set;}
        public string DynamicMinimum {get; set;}
        public string DynamicMaximumDelay { get; set; }
        public string DynamicMinimumDelay { get; set; }
        private IComparable min, max;
        private void getMinMaxForType()
        {

            min = null; max = null;
            /*
            if (TargetType == typeof(Int32))
            {
                min = int.MinValue; max = int.MaxValue;
            }
            else if (TargetType == typeof(Int16))
            {
                min = Int16.MinValue; max = Int16.MaxValue;
            }
            else if (TargetType == typeof(Int64))
            {
                min = Int64.MinValue; max = Int64.MaxValue;
            }
            else if (TargetType == typeof(UInt32))
            {
                min = UInt32.MinValue; max = UInt32.MaxValue;
            }
            else if (TargetType == typeof(UInt16))
            {
                min = UInt16.MinValue; max = UInt16.MaxValue;
            }
            else if (TargetType == typeof(UInt64))
            {
                min = UInt64.MinValue; max = UInt64.MaxValue;
            }
            else if (TargetType == typeof(byte))
            {
                min = byte.MinValue; max = byte.MaxValue;
            }
            else if (TargetType == typeof(sbyte))
            {
                min = sbyte.MinValue; max = sbyte.MaxValue;
            }
            else if (TargetType == typeof(decimal))
            {
                min = decimal.MinValue; max = decimal.MaxValue;
            }
            else if (TargetType == typeof(float))
            {
                min = float.MinValue; max = float.MaxValue;
            }
            else if (TargetType == typeof(double))
            {
                min = double.MinValue; max = double.MaxValue;
            }
            else if (TargetType == typeof(DateTime))
            {
                min = null; max = null;
            }
            */
            return;
        }
        private object getDelay(string delayRef, object model)
        {
            if (string.IsNullOrWhiteSpace(delayRef)) return null;
            PropertyAccessor delayProp = null;
            try
            {
                delayProp = new PropertyAccessor(model, delayRef, false);
            }
            catch
            {
                return null;
            }
            if (delayProp == null) return null;
            object res=delayProp.Value;
            if (TargetType == typeof(DateTime) && res != null) return ((TimeSpan)res).TotalMilliseconds;
            return res;
        }
        private object addDelay(string delayRef, ref IComparable value, object model)
        {
            if (string.IsNullOrWhiteSpace(delayRef) ) return null;
            PropertyAccessor delayProp = null;
            try
            {
                delayProp = new PropertyAccessor(model, delayRef, false);
            }
            catch
            {
                return null;
            }
            if (delayProp == null) return null;
            object toAdd = delayProp.Value;
            if (toAdd == null) return null;
            if (TargetType == typeof(Int32))
            {
                value = Convert.ToInt32(value) + Convert.ToInt32(toAdd);
            }
            else if (TargetType == typeof(Int16))
            {
                value = Convert.ToInt16(value) + Convert.ToInt16(toAdd);
            }
            else if (TargetType == typeof(Int64))
            {
                value = Convert.ToInt64(value) + Convert.ToInt64(toAdd);
            }
            else if (TargetType == typeof(UInt32))
            {
                value = Convert.ToUInt32(value) + Convert.ToUInt32(toAdd);
            }
            else if (TargetType == typeof(UInt16))
            {
                value = Convert.ToUInt16(value) + Convert.ToUInt16(toAdd);
            }
            else if (TargetType == typeof(UInt64))
            {
                value = Convert.ToUInt64(value) + Convert.ToUInt64(toAdd);
            }
            else if (TargetType == typeof(byte))
            {
                value = Convert.ToByte(value) + Convert.ToByte(toAdd);
            }
            else if (TargetType == typeof(sbyte))
            {
                value = Convert.ToSByte(value) + Convert.ToSByte(toAdd);
            }
            else if (TargetType == typeof(decimal))
            {
                value = Convert.ToDecimal(value) + Convert.ToDecimal(toAdd);
            }
            else if (TargetType == typeof(float))
            {
                value = Convert.ToSingle(value) + Convert.ToSingle(toAdd);
            }
            else if (TargetType == typeof(double))
            {
                value = Convert.ToDouble(value) + Convert.ToDouble(toAdd);
            }
            else if (TargetType == typeof(DateTime))
            {
                value = Convert.ToDateTime(value).Add((TimeSpan)toAdd);
                toAdd = ((TimeSpan)toAdd).TotalMilliseconds;
            }
            return toAdd ;
        }

        protected override ValidationResult  IsValid(object value, ValidationContext validationContext)
        {

            if (value == null) return ValidationResult.Success; ;
            
            IComparable toCheck = value as IComparable;
            if (toCheck == null) throw (new InvalidAttributeApplicationException(AnnotationsRsources.InvalidDynamicRangeApplication));
            
            if (validationContext != null)
            {
                if (Minimum != null &&  toCheck.CompareTo(Minimum)<0)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                if (Maximum != null && toCheck.CompareTo(Maximum) > 0)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                if (Minimum != null && toCheck.CompareTo(Minimum) < 0)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                if (Maximum != null && toCheck.CompareTo(Maximum) > 0)
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            if (!string.IsNullOrWhiteSpace(DynamicMinimum) &&  validationContext != null)
            {
                
                PropertyAccessor dynamicMinimumProp = 
                    new PropertyAccessor(validationContext.ObjectInstance, DynamicMinimum, false);
                
                if(dynamicMinimumProp!=null && dynamicMinimumProp.Value!= null)
                {
                    IComparable dMin = dynamicMinimumProp.Value as IComparable;

                    if (dMin == null) throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidLowerDynamicRange));
                    addDelay(DynamicMinimumDelay, ref dMin, validationContext.ObjectInstance);
                    
                    if (toCheck.CompareTo(dMin) < 0)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                }
            }
            if (!string.IsNullOrWhiteSpace(DynamicMaximum)  && validationContext != null)
            {
                PropertyAccessor dynamicMaximumProp =
                    new PropertyAccessor(validationContext.ObjectInstance, DynamicMaximum, false);

                if (dynamicMaximumProp != null && dynamicMaximumProp.Value != null)
                {

                    IComparable dMax = dynamicMaximumProp.Value as IComparable;
                    if (dMax == null) throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidLowerDynamicRange));

                    addDelay(DynamicMaximumDelay, ref dMax, validationContext.ObjectInstance);

                    if (toCheck.CompareTo(dMax) > 0)
                        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                    

                }
            }
            return ValidationResult.Success;
        }
        public object GetGlobalMaximum(object model, object globalModel, out string clientMaximum, out object maxDelay)
        {
            clientMaximum = null; maxDelay = null;
            if (model == null || string.IsNullOrWhiteSpace(DynamicMaximum)) return Maximum == null ? max : Maximum;
            IComparable constantMaximum = Maximum;
            IComparable dMax = null;
            PropertyAccessor dynamicMaximumProp = null;
            try
            {
                dynamicMaximumProp = new PropertyAccessor(model, DynamicMaximum, false);
            }
            catch
            {
                clientMaximum = DynamicMaximum;
                maxDelay = getDelay(DynamicMaximumDelay, model);
            }
            if (dynamicMaximumProp != null)
            {
                if (dynamicMaximumProp[typeof(MileStoneAttribute)].Length>0)
                {
                    if (dynamicMaximumProp.Value != null)
                    {
                        dMax = dynamicMaximumProp.Value as IComparable;
                        if (dMax == null) throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidLowerDynamicRange));

                        maxDelay = addDelay(DynamicMaximumDelay, ref dMax, model);
                    }
                }
                else
                {
                    clientMaximum = DynamicMaximum;
                    maxDelay = getDelay(DynamicMaximumDelay, model);
                }
            }
            object res=null;
            if (constantMaximum == null) res = dMax;
            else if (dMax == null) res = constantMaximum;
            else res = dMax.CompareTo(constantMaximum) > 0 ? constantMaximum : dMax;

            return res == null ? max : res;

        }
        public object GetGlobalMinimum(object model, object globalModel, out string clientMinimum, out object minDelay)
        {
            clientMinimum = null; minDelay = null;
            if (model == null || string.IsNullOrWhiteSpace(DynamicMinimum)) return Minimum == null ? min : Minimum;
            IComparable constantMinimum = Minimum;
            IComparable dMin = null;
            PropertyAccessor dynamicMinimumProp = null;
            try
            {
                dynamicMinimumProp = new PropertyAccessor(model, DynamicMinimum, false);
            }
            catch
            {
                clientMinimum = DynamicMinimum;
                minDelay = getDelay(DynamicMinimumDelay, model);
            }
            if (dynamicMinimumProp != null )
            {
                if (dynamicMinimumProp[typeof(MileStoneAttribute)].Length>0)
                {
                    if (dynamicMinimumProp.Value != null)
                    {
                        dMin = dynamicMinimumProp.Value as IComparable;

                        if (dMin == null) throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidLowerDynamicRange));
                        minDelay = addDelay(DynamicMinimumDelay, ref dMin, model);
                    }
                }
                else
                {
                    clientMinimum = DynamicMinimum;
                    minDelay = getDelay(DynamicMinimumDelay, model);
                }
            }
            object res=null;
            if (constantMinimum == null) res = dMin;
            else if (dMin == null) res = constantMinimum;
            else res = dMin.CompareTo(constantMinimum) < 0 ? constantMinimum : dMin;

            return res == null ? min : res ;
        }
        
        

    }
}

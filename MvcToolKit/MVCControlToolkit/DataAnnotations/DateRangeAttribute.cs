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
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Exceptions;
using System.Globalization;
using System.Web;

namespace MVCControlsToolkit.DataAnnotations
{
     
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DateRangeAttribute: System.ComponentModel.DataAnnotations.ValidationAttribute
    {
        
        public DateRangeAttribute(string message):
            base(message) 
        {
            RangeAction = RangeAction.Verify;
        }
        public DateRangeAttribute() :
            base(AnnotationsRsources.StandardError)
        {
            RangeAction = RangeAction.Verify;
        }
        public string SMinimum
        {
            set
            {
                if (processNowToday(value, false))
                {
                    return;
                }
                
                if(value==null)
                {
                    Minimum=null;
                    return;
                }
                DateTime res;
                if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out res))
                    Minimum = res;
                else throw (new FormatException(AnnotationsRsources.InvalidDateFormat));
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
                if (processNowToday(value, true))
                {
                    return;
                }
                
                if(value==null)
                {
                    Maximum=null;
                    return;
                }
                DateTime res;
                if(DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out res))
                   Maximum=res;
                else throw (new FormatException(AnnotationsRsources.InvalidDateFormat));
            }
            get
            {
                return null;
            }

        }
        public string SMaximumDelay
        {
            set
            {
                if (value == null)
                {
                    MaximumDelay = null;
                    return;
                }
                TimeSpan res;
                if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out res))
                    MaximumDelay = res;
                else throw (new FormatException(AnnotationsRsources.InvalidDateFormat));
            }
            get
            {
                return null;
            }

        }
        public string SMinimumDelay
        {
            set
            {
                if (value == null)
                {
                    MinimumDelay = null;
                    return;
                }
                TimeSpan res;
                if (TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out res))
                    MinimumDelay = res;
                else throw (new FormatException(AnnotationsRsources.InvalidDateFormat));
            }
            get
            {
                return null;
            }

        }
        public DateTime? Minimum{get; protected set;}
        public DateTime? Maximum{get; protected set;}
        public TimeSpan? MinimumDelay { get; protected set; }
        public TimeSpan? MaximumDelay { get; protected set; }
        public string DynamicMinimumDelay { get; set; }
        public string DynamicMaximumDelay { get; set; }
        public RangeAction RangeAction { get; set; }
        public string DynamicMaximum {get; set;}
        public string DynamicMinimum {get; set;}
        object min, max;
        private long toleranceMin = 0;
        private long toleranceMax = 0;
        private bool processNowToday(string dateString, bool max)
        {
            DateTime currDate;
            if (dateString.StartsWith("Now"))
            {
                currDate = DateTime.Now;
                dateString = dateString.Substring(3);
            }
            else if (dateString.StartsWith("Today"))
            {
                currDate = DateTime.Today;
                dateString = dateString.Substring(5);
            }
            else
            {
                return false;
            }
            bool buildingNumber = false;
            long numberBeingBuild=0;
            bool toInvert= false;
            foreach (char c in dateString)
            {
                if (c == '+')
                {
                    toInvert = false;
                    numberBeingBuild = 0;
                    buildingNumber = true;
                }
                else if (c == '-')
                {
                    toInvert = true;
                    numberBeingBuild = 0;
                    buildingNumber = true;
                }
                else if (buildingNumber)
                {
                    if (c.CompareTo('0') >= 0 && c.CompareTo('9') <= 0)
                    {
                        numberBeingBuild = numberBeingBuild * 10 + int.Parse(c.ToString());
                    }
                    else
                    {
                        buildingNumber=false;
                        if (toInvert) numberBeingBuild = -numberBeingBuild;
                        switch (c)
                        {
                            case 's':
                                currDate = currDate.AddSeconds(numberBeingBuild);
                                break;
                            case 'm': 
                                currDate = currDate.AddMinutes(numberBeingBuild);
                                break;
                            case 'h':
                                currDate = currDate.AddHours(numberBeingBuild);
                                break;
                            case 'd':
                                currDate = currDate.AddDays(numberBeingBuild);
                                break;
                            case 'M':
                                currDate = currDate.AddMonths((int)numberBeingBuild);
                                break;
                            case 'y':
                                currDate = currDate.AddYears((int)numberBeingBuild);
                                break;
                            case 'T':
                                if (HttpContext.Current.Items.Contains("CurrentTimeShift"))
                                {
                                    currDate = currDate.AddHours((int)HttpContext.Current.Items["CurrentTimeShift"]);
                                }
                                break;
                            case 't':
                                if (max)
                                    toleranceMax = numberBeingBuild > 0 ? numberBeingBuild : -numberBeingBuild;
                                else
                                    toleranceMin = numberBeingBuild > 0 ? numberBeingBuild : -numberBeingBuild;
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
            }
            if (max) Maximum = currDate;
            else Minimum = currDate;
            return true;
        }
        public void RetriveDynamicDelays(object o, string addPrefix=null)
        {
            
            if (DynamicMinimumDelay != null)
            {
                string correctedDynamicMinimumDelay=DynamicMinimumDelay.Trim();
                if (!string.IsNullOrWhiteSpace(addPrefix)) correctedDynamicMinimumDelay =
                     addPrefix.Trim() + "." + correctedDynamicMinimumDelay;
                PropertyAccessor dynamicMinimumDelay =
                    new PropertyAccessor(o, correctedDynamicMinimumDelay, false);

                 if (dynamicMinimumDelay != null && dynamicMinimumDelay.Value != null)
                 {
                     if (dynamicMinimumDelay.Property.PropertyType == typeof(TimeSpan))
                     {
                         MinimumDelay = ((TimeSpan)dynamicMinimumDelay.Value);
                     }
                     else if (dynamicMinimumDelay.Property.PropertyType == typeof(Nullable<TimeSpan>))
                     {
                         MinimumDelay = ((Nullable<TimeSpan>)dynamicMinimumDelay.Value).Value;

                     }
                     else
                         throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidLowerDynamicDelay));
                 }
            }
            if (DynamicMaximumDelay != null)
            {
                string correctedDynamicMaximumDelay = DynamicMaximumDelay.Trim();
                if (!string.IsNullOrWhiteSpace(addPrefix)) correctedDynamicMaximumDelay =
                     addPrefix.Trim() + "." + correctedDynamicMaximumDelay;
                PropertyAccessor dynamicMaximumDelay =
                   new PropertyAccessor(o, correctedDynamicMaximumDelay, false);

                if (dynamicMaximumDelay != null && dynamicMaximumDelay.Value != null)
                {
                    if (dynamicMaximumDelay.Property.PropertyType == typeof(TimeSpan))
                    {
                        MaximumDelay = (TimeSpan)dynamicMaximumDelay.Value;
                    }
                    else if (dynamicMaximumDelay.Property.PropertyType == typeof(Nullable<TimeSpan>))
                    {
                        MaximumDelay = (Nullable<TimeSpan>)dynamicMaximumDelay.Value;

                    }
                    else
                        throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidUpperDynamicDelay));
                }
            }
        }

        protected override ValidationResult  IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            if(validationContext!=null)  RetriveDynamicDelays(validationContext.ObjectInstance);
            DateTime toCheck = default(DateTime);
            if (value.GetType() == typeof(DateTime)) toCheck = (DateTime)value;
            else if (value.GetType() == typeof(Nullable<DateTime>))
            {
                Nullable<DateTime> nToCheck = value as Nullable<DateTime>;
                if (nToCheck.HasValue) toCheck = nToCheck.Value;
                else return ValidationResult.Success;
            }
            else
            {
                throw (new InvalidAttributeApplicationException(AnnotationsRsources.InvalidDateRangeApplication));
            }
            if (validationContext != null)
            {
                if (Minimum != null && Minimum.HasValue && toCheck < Minimum.Value.AddMinutes(-toleranceMin))
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                if (Maximum != null && Maximum.HasValue && toCheck > Maximum.Value.AddMinutes(toleranceMax))
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            else
            {
                if (Minimum != null && Minimum.HasValue && toCheck < Minimum.Value.AddMinutes(-toleranceMin))
                    return new ValidationResult(FormatErrorMessage("Date or Time"));
                if (Maximum != null && Maximum.HasValue && toCheck > Maximum.Value.AddMinutes(toleranceMax))
                    return new ValidationResult(FormatErrorMessage("Date or Time"));
            }
            if (!string.IsNullOrWhiteSpace(DynamicMinimum) &&  validationContext != null)
            {
                
                PropertyAccessor dynamicMinimumProp = 
                    new PropertyAccessor(validationContext.ObjectInstance, DynamicMinimum, false);
                
                if(dynamicMinimumProp!=null && dynamicMinimumProp.Value!= null)
                {
                    TimeSpan delay = new TimeSpan(0);
                    if (MinimumDelay != null && MinimumDelay.HasValue) delay = MinimumDelay.Value;
                    if (dynamicMinimumProp.Property.PropertyType == typeof(DateTime))
                    {
                        if (((DateTime)dynamicMinimumProp.Value).Add(delay) > toCheck)
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                    else if (dynamicMinimumProp.Property.PropertyType == typeof(Nullable<DateTime>))
                    {
                        Nullable<DateTime> min = dynamicMinimumProp.Value as Nullable<DateTime>;
                        if (min.HasValue) min = min.Value.Add(delay);
                        if (min.HasValue && min > toCheck)
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                    }
                    else
                        throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidLowerDynamicRange));

                }
            }
            if (!string.IsNullOrWhiteSpace(DynamicMaximum) && validationContext != null)
            {
                PropertyAccessor dynamicMaximumProp =
                    new PropertyAccessor(validationContext.ObjectInstance, DynamicMaximum, false);

                if (dynamicMaximumProp != null && dynamicMaximumProp.Value != null)
                {
                    TimeSpan delay = new TimeSpan(0);
                    if (MaximumDelay != null && MaximumDelay.HasValue) delay = MaximumDelay.Value;
                    if (dynamicMaximumProp.Property.PropertyType == typeof(DateTime))
                    {
                        if (((DateTime)dynamicMaximumProp.Value).Add(delay) < toCheck)
                        {
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                        }
                    }
                    else if (dynamicMaximumProp.Property.PropertyType == typeof(Nullable<DateTime>))
                    {
                        Nullable<DateTime> max = dynamicMaximumProp.Value as Nullable<DateTime>;
                        if (max.HasValue) max = max.Value.Add(delay);
                        if (max.HasValue && max < toCheck)
                            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));

                    }
                    else
                        throw (new InvalidDynamicRangeException(AnnotationsRsources.InvalidUpperDynamicRange));

                }
            }
            return ValidationResult.Success;
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
            return delayProp.Value;
        }
        private object addDelay(string delayRef, ref IComparable value, object model)
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
            object toAdd = delayProp.Value;
            if (toAdd == null) return null;
            
            value = ((DateTime)value).Add((TimeSpan)toAdd);

            return ((TimeSpan)toAdd).TotalMilliseconds; 
        }
        private void getMinMaxForType()
        {
            min = null; max = null;
            
            return;
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
            object res = null;
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
            if (dynamicMinimumProp != null)
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
            object res = null;
            if (constantMinimum == null) res = dMin;
            else if (dMin == null) res = constantMinimum;
            else res = dMin.CompareTo(constantMinimum) < 0 ? constantMinimum : dMin;

            return res == null ? min : res;
        }

    }
}

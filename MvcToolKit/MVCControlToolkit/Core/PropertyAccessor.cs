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
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVCControlsToolkit.Core
{
    public class PropertyAccessor
    {
        private object destination;
        private PropertyInfo property;
        private PropertyInfo metaProperty;
        private bool metaDataOnly=false;
        private bool createWhenNeeded = true;
        public object Value
        {
            set
            {
                if (metaDataOnly) return;
                property.SetValue(destination, value, new object[0]);
            }
            get
            {
                if (metaDataOnly) return null;
                 return property.GetValue(destination, new object[0]);
            }
        }
        public PropertyInfo Property
        {
            get
            {
                return property;
            }
        }
        public object[] this[Type attributetType]
        {
            get
            {
                if (property == null) return null;
                object[] res0= property.GetCustomAttributes(attributetType, true);
                if (metaProperty == null) return res0;
                object[] res1 = metaProperty.GetCustomAttributes(attributetType, true);

                if (res1 == null || res1.Length == 0) return res0;
                else return res1;
                
                 
            }
        }
        protected DisplayNameAttribute Display
        {
            get
            {
                object[] displays = this[typeof(DisplayNameAttribute)];
                if (displays == null || displays.Length == 0) return null;
                else return displays[0] as DisplayNameAttribute;
            }
        }
        public string DisplayName
        {
            get
            {
                DisplayNameAttribute display = Display;
                if (display == null || string.IsNullOrWhiteSpace(display.DisplayName)) return property.Name;
                else return display.DisplayName;
            }
        }
        public PropertyAccessor(string expression, Type type)
        {
            metaDataOnly = true;
            createWhenNeeded = false;
            Initializer(null, expression, type);
        }
        public PropertyAccessor(object destination, string expression, bool createWhenNeeded=true)
        {
            this.createWhenNeeded=createWhenNeeded;
            Initializer(destination, expression, null);
        }
        protected void Initializer(object destination, string expression, Type type)
        {
            
            if (string.IsNullOrWhiteSpace(expression)) throw(new ArgumentNullException(expression));
            if (!metaDataOnly && expression.Contains("[")) throw new NotSupportedException(string.Format(Resources.NotSupportedEnumerables, "PropertySetter"));
            
            Type currType;
            if (metaDataOnly)
                currType = type;
            else
                currType = destination.GetType();
            PropertyInfo currProperty=null;
            int index = 0;
            string[] fields=expression.Split(new string[]{"."}, StringSplitOptions.None);
            bool locked = false;
            foreach(string roughField in fields )
            {
                string field=roughField;
                if (metaDataOnly && field.IndexOf('[') >=0) field = field.Substring(0, field.IndexOf('['));
                currProperty = currType.GetProperty(field);
                if (currProperty == null)
                {
                    if (locked || index >= fields.Length - 1)
                        throw new MVCControlsToolkit.Exceptions.PropertyNotFoundException(field, currType);
                    else
                    {
                        index++;
                        continue;
                    }
                }
                else locked = true;
                if (index < fields.Length-1)
                {
                    Type newType = currProperty.PropertyType;
                    if (!metaDataOnly)
                    {
                        object newValue = currProperty.GetValue(destination, new object[0]);
                        if (newValue == null)
                        {
                            if (createWhenNeeded)
                            {
                                if (newType.IsClass)
                                {
                                    ConstructorInfo ci = newType.GetConstructor(new Type[0]);
                                    if (ci == null) throw new NotSupportedException(string.Format(Resources.NoConstructor, newType.Name));
                                    object newDestination = ci.Invoke(new object[0]);

                                    currProperty.SetValue(destination, newDestination, new object[0]);
                                    destination = newDestination;
                                    currType = newType;

                                }
                                else if (newType.IsInterface) throw new NotSupportedException(string.Format(Resources.NotSupportedInterface, "PropertySetter"));
                            }
                            else
                            {
                                destination = null;
                                currType = newType;
                                metaDataOnly = true;
                            }
                        }
                        else
                        {
                            currType = newType;
                            destination = newValue;
                        }
                    }
                    else
                        currType = newType;
                }
                else
                {
                    this.destination = destination;
                    property = currProperty;
                    object[] metas = currProperty.DeclaringType.GetCustomAttributes(typeof(MetadataTypeAttribute), true);
                    MetadataTypeAttribute meta = null;
                    if (metas != null && metas.Length > 0) meta = metas[0] as MetadataTypeAttribute;
                    if (meta != null)
                        metaProperty=meta.MetadataClassType.GetProperty(property.Name);
                    break;
                }
                index++;
            }
        }
    }
}

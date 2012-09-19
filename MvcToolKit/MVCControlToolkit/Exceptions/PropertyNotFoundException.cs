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
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Exceptions
{
    public class PropertyNotFoundException: Exception
    {
        public PropertyNotFoundException(string property, Type objectType):
            base(string.Format(Resources.PropertyNotFound, objectType.GetType(), property))
        {
            _property = property;
            _type = objectType;
            
        }
        string _property;
        Type _type;

        public string PropertyName
        {
            get { return _property; }
        }
        public Type Type
        {
            get { return _type;}
        }
    }
}

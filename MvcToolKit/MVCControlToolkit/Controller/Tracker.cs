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

namespace MVCControlsToolkit.Controller
{
    [Serializable]
    public class Tracker<T>
        
    {
        public bool Changed { get; set;}
        public T Value { set; get; }
        public T OldValue { set; get; }
        public Tracker(){}
        public Tracker(T value)
        {
            OldValue = Value = value;
        }
        public Tracker(object oldValue, object newValue, bool changed)
        {
            if (oldValue == null) OldValue = default(T);
            else OldValue = (T)oldValue;
            if (newValue == null) Value = default(T);
            else Value = (T)newValue;
            Changed = changed;
        }
        public T GetForEdit()
        {
            if (Value == null) return default(T);
            Changed = true;
            return Value;
        }
        public void Cancel()
        {
            if (!Changed) return;
            Changed = false;
            Value = OldValue;
        }
        public void Confirm()
        {
            if (!Changed) return;
            OldValue = Value;
            Changed = false;
        }

    }
}

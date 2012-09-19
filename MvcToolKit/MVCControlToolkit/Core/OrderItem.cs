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

namespace MVCControlsToolkit.Core
{
    internal class OrderItem<TValue> : IComparable<OrderItem<TValue>>
        where TValue : IComparable
    {
        public int Place { get; set; }
        public TValue Value { get; set; }



        public int CompareTo(OrderItem<TValue> other)
        {
            return ((IComparable)Value).CompareTo(other.Value);
        }
    }
}

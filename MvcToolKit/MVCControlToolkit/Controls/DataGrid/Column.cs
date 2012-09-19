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
using System.Linq.Expressions;
using System.Text;

namespace MVCControlsToolkit.Controls
{
    public class Column
    {
        public dynamic Field {get; set;}
        public FieldFeatures Features { get; set; }
        public object DispalyTemplate { get; set; }
        public object EditTemplate { get; set; }
        public string ColumnHeader { get; set; }
    }
}

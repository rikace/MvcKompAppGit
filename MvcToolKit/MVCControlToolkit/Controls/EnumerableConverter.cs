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
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Collections;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class EnumerableConverter<T>: IDisplayModel
    {
        public List<T> Items
        {
            get;
            set;
        }
        public object ExportToModel(Type targetType, params object[] context)
        {
            return EnumerableHelper.CreateFrom<T>(targetType, Items, Items.Count);
        }

        public void ImportFromModel(object model, params object[] context)
        {
            Items = EnumerableHelper.CreateFrom<T>(typeof(List<T>), (IEnumerable)model, 0) as List<T>;
        }
    }
}

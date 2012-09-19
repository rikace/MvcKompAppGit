﻿/* ****************************************************************************
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
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class GridDescriptionBase
    {
        public GridFeatures Features { get; set; }
        public List<Column> Fields { get; set; }
        public FieldsToTrack GetToTrack()
        {
            FieldsToTrack result= new FieldsToTrack();
            result.Fields = new List<LambdaExpression>();
            if (Fields == null) return result;
            foreach (Column col in Fields)
            {
                result.Fields.Add(col.Field);
            }
            return result;
        }
    }
    
    public class GridDescription:GridDescriptionBase
    {
        public dynamic ToShow {get; set;}
        public dynamic ToOrder { get; set; }
        public string Title { get; set; }
        public dynamic HtmlHelper { get; set; }
        public dynamic Page { get; set; }
        public dynamic PrevPage { get; set; }
        public dynamic PageCount { get; set; }
        public dynamic Filter { get; set; }

    }


}

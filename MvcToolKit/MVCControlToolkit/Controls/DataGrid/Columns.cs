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
using System.Web.Mvc;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    public class FieldsToTrack
    {
        public List<LambdaExpression> Fields { get; set; }
        public string FieldsToUpdate()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append("'");
            foreach(LambdaExpression exp in Fields){
                if (!first) sb.Append(',');
                else first = false;
                sb.Append(BasicHtmlHelper.IdFromName(ExpressionHelper.GetExpressionText(exp)));
            }
            sb.Append("'");
            return sb.ToString();
        }
        public string FieldsToUpdateList()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append("[");
            foreach (LambdaExpression exp in Fields)
            {
                if (!first) sb.Append(", ");
                else first = false;
                sb.Append("'");
                sb.Append(BasicHtmlHelper.IdFromName(ExpressionHelper.GetExpressionText(exp)));
                sb.Append("'");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
    public class FieldsToTrack<T> : FieldsToTrack
    {
        
        public FieldsToTrack<T> Add<F>(
            Expression<Func<T, F>> field)
        {
            if (Fields == null) Fields = new List<LambdaExpression>();
            Fields.Add(field);
            return this;
        }

    }
    public class Columns<T>
    {
        public List<Column> Fields { get; set; }
        public Columns<T> Add<F>
            (Expression<Func<T, F>> field, 
            FieldFeatures features= FieldFeatures.None, 
            object editTemplate=null,
            object displayTemplate=null,
            string overrideName=null){
            if (field == null) throw (new ArgumentException("field"));
            if (Fields == null) Fields = new List<Column>();
            Fields.Add(new Column
            {
                Field=field,
                Features=features,
                EditTemplate = editTemplate,
                DispalyTemplate = displayTemplate,
                ColumnHeader = overrideName

            });
            return this;
        }

    }
}

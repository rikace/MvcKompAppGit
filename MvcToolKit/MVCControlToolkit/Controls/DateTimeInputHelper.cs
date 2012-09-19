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
using System.Collections;

namespace MVCControlsToolkit.Controls
{
    public static class DateTimeInputHelper
    {
        public static DateTimeInput<VM> DateTimeFor<VM>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, DateTime>> expression,
            DateTime emptyDate, bool dateInCalendar = false, IDictionary<string, object> attributeExtensions=null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            DateTime value=default(DateTime);
            DateTime? nValue=null ;
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            }
            catch { }
            if (value != default(DateTime))
                nValue = value;
            else
                nValue = emptyDate;

            string fullPropertyPath =
                      ExpressionHelper.GetExpressionText(expression);

            return new DateTimeInput<VM>(htmlHelper, fullPropertyPath, nValue, dateInCalendar, attributeExtensions);
        }
        public static DateTimeInput<VM> DateTime<VM>(this HtmlHelper<VM> htmlHelper, string name,
            DateTime value, bool dateInCalendar = false, IDictionary<string, object> attributeExtensions=null)
        {
            if (name == null) throw (new ArgumentNullException("name"));
            
            return new DateTimeInput<VM>(htmlHelper, name, value, dateInCalendar, attributeExtensions);
        }

        public static DateTimeInput<VM> DateTimeFor<VM>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, Nullable<DateTime>>> expression,
            bool dateInCalendar = false, IDictionary<string, object> attributeExtensions = null,  DateTime? emptyDate=null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            DateTime? nValue=null;
            if (emptyDate == null) emptyDate = System.DateTime.Now;
            try
            {
                nValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            }
            catch { }
            if (nValue == null) nValue = emptyDate;
            string fullPropertyPath =
                      ExpressionHelper.GetExpressionText(expression);
            return new DateTimeInput<VM>(htmlHelper, fullPropertyPath, nValue, dateInCalendar, attributeExtensions);
        }

        public static DateTimeInput<VM> DateTime<VM>(this HtmlHelper<VM> htmlHelper, string name, Nullable<DateTime> value,
            bool dateInCalendar = false, IDictionary<string, object> attributeExtensions = null)
        {
            if (name == null) throw (new ArgumentNullException("name"));

            return new DateTimeInput<VM>(htmlHelper, name, value, dateInCalendar, attributeExtensions);
        }
    }
}

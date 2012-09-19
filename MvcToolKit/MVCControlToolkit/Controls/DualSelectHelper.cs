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
    public static class DualSelectHelper
    {
        public static DualSelect<TModel, IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>
            DualSelectFor<TModel, TChoiceItem, TValue, TDisplay>
                (this HtmlHelper<TModel> htmlHelper,
                Expression<Func<TModel, IEnumerable<TValue>>> expression,
                ChoiceList<TChoiceItem, TValue, TDisplay> choiceList,
                IDictionary<string, object> attributeExtensions=null)
        {
            IEnumerable<TValue> values = default(IEnumerable<TValue>);
            if (expression == null) throw (new ArgumentNullException("expression"));
            try
            {
                values = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            if (choiceList == null) throw (new ArgumentNullException("choiceList"));
            if (values == null) values = new List<TValue>();
            var fullPropertyPath =
                  
                      ExpressionHelper.GetExpressionText(expression);
            return new DualSelect<TModel, IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>()
            {
                CurrHtmlHelper=htmlHelper,
                Prefix = fullPropertyPath,
                Value = values,
                CurrChoiceList=choiceList,
                AttributeExtensions = attributeExtensions
            };
        }
    }
}

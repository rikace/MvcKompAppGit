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
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls.Bindings;

namespace MVCControlsToolkit.Controls
{
    public static class JsonHelpers
    {
        public static MvcHtmlString JsonModel<M,T>(this HtmlHelper<M> htmlHelper, string name, T value, IDictionary<string, object> htmlAttributes=null, bool translateBack = false)
        {
            if (string.IsNullOrWhiteSpace(name)) throw(new ArgumentNullException("name"));
            if (translateBack)
            {
                RenderInfo<T> renderInfo= new RenderInfo<T>(
                    htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name),
                    name,
                    string.Empty,
                    value);
                var h=htmlHelper.RenderWith(
                    htmlHelper.InvokeTransform(
                        renderInfo,
                        new SipleModelTranslator<T>()));
                return h.HiddenFor(m => m.JSonModel, htmlAttributes);
            }
            else
            {
                return htmlHelper.Hidden(name, value==null ? null : BasicHtmlHelper.ClientEncode(value), htmlAttributes);
            }
        }
        public static MvcHtmlString JsonModelFor<M, T>(this HtmlHelper<M> htmlHelper, Expression<Func<M, T>> expression, IDictionary<string, object> htmlAttributes=null, bool translateBack = false)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            T value = default(T);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            return JsonModel(htmlHelper, ExpressionHelper.GetExpressionText(expression), value, htmlAttributes, translateBack);
        }
    }
}

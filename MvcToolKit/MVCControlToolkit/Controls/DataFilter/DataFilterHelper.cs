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
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls.DataFilter;


namespace MVCControlsToolkit.Controls
{
    public static class DataFilterHelper
    {
        public static MvcHtmlString DataFilter<M, TData, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, Expression<Func<TData, bool>>>> expression,
            T filterDescription,
            object filterTemplate)
            where T : class, IFilterDescription<TData>
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (filterDescription == null) throw (new ArgumentNullException("filterDescription"));
            if (filterTemplate == null) throw (new ArgumentNullException("filterTemplateName"));

            string fieldPartialName = ExpressionHelper.GetExpressionText(expression);
            string fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldPartialName);

            Expression<Func<TData, bool>> fieldValue = null;
            try
            {
                fieldValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            FilterContext<T, TData, M> filterContext = new FilterContext<T, TData, M>(filterDescription);
            filterContext.ImportFromModel(fieldValue, fieldName, htmlHelper.ViewContext.RequestContext.HttpContext.Items);
            return filterContext.Render(htmlHelper, fieldPartialName, fieldName, filterTemplate);
        }

        public static HtmlHelper<T> DataFilterBuilder<M, TData, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, Expression<Func<TData, bool>>>> expression,
            T filterDescription)
            where T : class, IFilterDescription<TData>
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (filterDescription == null) throw (new ArgumentNullException("filterDescription"));
            

            string fieldPartialName = ExpressionHelper.GetExpressionText(expression);
            string fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldPartialName);

            Expression<Func<TData, bool>> fieldValue = null;
            try
            {
                fieldValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            FilterContext<T, TData, M> filterContext = new FilterContext<T, TData, M>(filterDescription);
            filterContext.ImportFromModel(fieldValue, fieldName, htmlHelper.ViewContext.RequestContext.HttpContext.Items);
            return filterContext.GetHelper(htmlHelper, fieldPartialName, fieldName);
        }
        /*
        public static DataFilterBuilder<T, TData, M> DataFilterBuilderFarm<M, TData, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, Expression<Func<TData, bool>>>> expression,
            T filterDescription)
            where T : class, IFilterDescription<TData>
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (filterDescription == null) throw (new ArgumentNullException("filterDescription"));

            string fieldPartialName = ExpressionHelper.GetExpressionText(expression);
            string fieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldPartialName);

            Expression<Func<TData, bool>> fieldValue = null;
            try
            {
                fieldValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            FilterContext<T, TData, M> filterContext = new FilterContext<T, TData, M>(filterDescription);
            filterContext.ImportFromModel(fieldValue, fieldName, htmlHelper.ViewContext.RequestContext.HttpContext.Items);
            htmlHelper.ViewContext.Writer.Write(
                BasicHtmlHelper.RenderDisplayInfo(htmlHelper, filterContext.GetType(), fieldPartialName));
            return new DataFilterBuilder<T, TData, M>(filterContext, BasicHtmlHelper.AddField(fieldName, "$.FilterDescription"));
        }*/
    }
}

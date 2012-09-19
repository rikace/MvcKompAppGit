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
using MVCControlsToolkit.Core;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCControlsToolkit.Controls.DataFilter
{
    public class FilterContext<T, TData, M>: IDisplayModel
        where T: class, IFilterDescription<TData>
    {
        public FilterContext()
        {
        }
        public FilterContext(T filterDescription)
        {
            FilterDescription = filterDescription;
        }
        public T FilterDescription { get; set; }
       
        public object ExportToModel(Type TargetType, params object[] context)
        {
            
            if (context != null && context.Length > 1)
            {
                IDictionary dict = (context[1] as IDictionary);
                if (dict != null) dict[context[0] as string]=this.FilterDescription;

            }
            return FilterDescription.GetExpression();
        }

        public void ImportFromModel(object model, params object[] context)
        {
            Expression<Func<TData, bool>>  currExpression = model as Expression<Func<TData, bool>>;
            if (currExpression == null) return;
            else if(context != null && context.Length>1)
            {
                IDictionary dict = (context[1] as IDictionary);
                string key = context[0] as string;
                if (dict != null && dict.Contains(key)) FilterDescription = dict[key] as T;
            }
        }
        internal HtmlHelper<T> GetHelper(HtmlHelper<M> htmlHelper, string partialPrefix, string prefix)
        {
            htmlHelper.ViewContext.Writer.Write(
                BasicHtmlHelper.RenderDisplayInfo(htmlHelper, this.GetType(), partialPrefix));
            ViewDataDictionary<T> dataDictionary =
               new ViewDataDictionary<T>(FilterDescription);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.FilterDescription");
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            return new TemplateInvoker<T>().BuildHelper(htmlHelper, dataDictionary);
        }
        internal  MvcHtmlString Render(HtmlHelper<M> htmlHelper, string partialPrefix, string prefix, object template)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BasicHtmlHelper.RenderDisplayInfo(htmlHelper, this.GetType(), partialPrefix));
            ViewDataDictionary<T> dataDictionary =
               new ViewDataDictionary<T>(FilterDescription);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.FilterDescription");
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            sb.Append(new TemplateInvoker<T>(template).Invoke<M>(htmlHelper, dataDictionary));
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}

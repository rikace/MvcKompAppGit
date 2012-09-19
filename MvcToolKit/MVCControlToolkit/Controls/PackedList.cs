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
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace MVCControlsToolkit.Controls
{
    public class PackedList<TEnumerable, TValue>:IDisplayModel
        where TEnumerable:IEnumerable
    {
        
        public string Separator { get; set; }
        public string PackedValue { get; set; }

        public object ExportToModel(Type targetType, params object[] context)
        {
            List<TValue> res = new List<TValue>();
            if (!string.IsNullOrEmpty(PackedValue))
            {
                PackedValue = PackedValue.Replace(@"@" + Separator, @"\@").Replace(@"\" + Separator, "@@");
                string correctedSeparator = Separator.Trim();
                if (correctedSeparator.Length == 0) correctedSeparator = " ";
                string[] split = PackedValue.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string x in split)
                {
                    if (typeof(IConvertible).IsAssignableFrom(typeof(TValue)))
                        res.Add((TValue)(x.Trim().Replace("@@", Separator).Replace(@"\@", "@").Replace(@"\\", @"\") as IConvertible).ToType(typeof(TValue), CultureInfo.CurrentCulture));
                    else
                        res.Add((TValue)(TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(x.Trim().Replace("@@", Separator).Replace(@"\@", "@").Replace(@"\\", @"\"))));

                }
            }
            return EnumerableHelper.CreateFrom<TValue>(targetType, res, res.Count);
        }

        public void ImportFromModel(object model, params object[] context)
        {
            Separator = context[0] as string;
            IEnumerable input= model as IEnumerable;
            if (input == null)
            {
                PackedValue = string.Empty;
                return;
            }

            StringBuilder sb = new StringBuilder();

            bool start = true;

            foreach (object o in input)
            {
                if (start) start = false;
                else
                {
                    sb.Append(Separator);
                    
                }

                TValue item = default(TValue);
                if (o != null) item = (TValue)o;
                sb.Append(item.ToString().Replace(@"\", @"\\").Replace(Separator, @"\"+Separator));
            }
            PackedValue = sb.ToString();
        }
    }

    public static class PackedListHelper
    {
        public static MvcHtmlString PackedListFor<TModel, TValue>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, IEnumerable<TValue>>> expression, string separator, object attributes = null, bool useHidden = false, bool useTemplate = false, object template = null)
        {

            var fullPropertyPath =
                    htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                        ExpressionHelper.GetExpressionText(expression));
            var propertyPath = ExpressionHelper.GetExpressionText(expression);
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (separator == null) throw (new ArgumentNullException("separator"));
            if (attributes == null) attributes = new { };
            IEnumerable<TValue> values = null;
            try
            {
                values = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            if (values == null) values = new List<TValue>();
            PackedList<IEnumerable<TValue>, TValue> displayModel = new PackedList<IEnumerable<TValue>, TValue>();
            
            displayModel.ImportFromModel(values, new object[] { separator });
            
            StringBuilder sb = new StringBuilder();
            sb.Append(BasicHtmlHelper.RenderDisplayInfo(htmlHelper, 
                typeof(PackedList<IEnumerable<TValue>, TValue>),
                propertyPath));


            sb.Append(htmlHelper.Hidden(BasicHtmlHelper.AddField(propertyPath, "$.Separator"), separator)).ToString();

            if (useTemplate)
            {
                if (template == null) template = typeof(PackedList<IEnumerable<TValue>, TValue>).Name;

                ViewDataDictionary<PackedList<IEnumerable<TValue>, TValue>> dataDictionary =
                    new ViewDataDictionary<PackedList<IEnumerable<TValue>, TValue>>(displayModel);
                dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(fullPropertyPath, "$");

                sb.Append(new TemplateInvoker<PackedList<IEnumerable<TValue>, TValue>>(template)
                    .Invoke(htmlHelper, dataDictionary));


            }
            else
            {
                if(useHidden)
                    sb.Append(htmlHelper.Hidden(
                        BasicHtmlHelper.AddField(propertyPath, "$.PackedValue"),
                        displayModel.PackedValue,
                        attributes));
                else
                    sb.Append(htmlHelper.TextBox(
                            BasicHtmlHelper.AddField(propertyPath, "$.PackedValue"),
                            displayModel.PackedValue,
                            attributes));
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}

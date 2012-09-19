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
using System.Globalization;

namespace MVCControlsToolkit.Controls
{
    public class CheckBoxList<TEnumerable, TChoiceItem, TValue, TDisplay> : IDisplayModel
        where TValue : IComparable
        where TEnumerable : IEnumerable
    {
        public CheckBoxListItem<TValue>[] Items { get; set; } 
        public object ExportToModel(Type targetType, params object[] context)
        {
            List<TValue> res = new List<TValue>();
            
            foreach (CheckBoxListItem<TValue> x in Items)
            {
                if (x.Selected) res.Add(x.Code);
               
            }
            return EnumerableHelper.CreateFrom<TValue>(targetType, res, res.Count);
        }


        public void ImportFromModel(object model, params object[] context)
        {
            IEnumerable input = model as IEnumerable;
            List<TValue> inputList;

            if (input == null)
                inputList = new List<TValue>();
            else
            {
                inputList = EnumerableHelper.CreateFrom<TValue>(typeof(List<TValue>), input, 0) as List<TValue>;

                inputList.Sort();

                ChoiceList<TChoiceItem, TValue, TDisplay> choiceList = context[0] as ChoiceList<TChoiceItem, TValue, TDisplay>;

                List<CheckBoxListItem<TValue>> items = new List<CheckBoxListItem<TValue>>();
                List<OrderItem<TValue>> orderer = new List<OrderItem<TValue>>();

                int index = 0;

                foreach (TChoiceItem item in choiceList.Items)
                {
                    CheckBoxListItem<TValue> checkBoxListItem = new CheckBoxListItem<TValue>();
                    OrderItem<TValue> orderItem = new OrderItem<TValue>();
                    checkBoxListItem.Code = choiceList.ValueSelector(item);
                    checkBoxListItem.Label = choiceList.DisplaySelector(item).ToString();

                    if (choiceList.LabelAttributesSelector != null)
                        checkBoxListItem.LabelAttributes = choiceList.LabelAttributesSelector(item);
                    else
                        checkBoxListItem.LabelAttributes = new { };
                    if (choiceList.DisplayAttributesSelector != null)
                        checkBoxListItem.DisplayAttributes = choiceList.DisplayAttributesSelector(item);
                    else
                        checkBoxListItem.DisplayAttributes = new { };
                    items.Add(checkBoxListItem);

                    orderItem.Value = checkBoxListItem.Code;
                    orderItem.Place = index;
                    orderer.Add(orderItem);
                    index++;
                }
                Items = items.ToArray();
                orderer.Sort();
                IEnumerator<OrderItem<TValue>> ordererIterator = (orderer as IEnumerable<OrderItem<TValue>>).GetEnumerator();
                ordererIterator.Reset();
                ordererIterator.MoveNext();
                foreach (TValue selectedValue in inputList)
                {
                    while (((IComparable)selectedValue).CompareTo(ordererIterator.Current.Value) > 0) ordererIterator.MoveNext();
                    if (((IComparable)selectedValue).CompareTo(ordererIterator.Current.Value) == 0)
                    {
                        Items[ordererIterator.Current.Place].Selected = true;
                    }
                }

            }
        }
    }
    public class CheckBoxListItem<TValue>
    {
        public bool Selected{get;set;}
        public string Label { get; set; }
        public TValue Code {get;set;}
        public object DisplayAttributes { get; set; }
        public object LabelAttributes { get; set; }

    }

    

    public static class CheckBoxListHelper
    {
        public static MvcHtmlString ThemedChoiceListFor<TModel, TChoiceItem, TValue, TDisplay>
            (
           this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, IEnumerable<TValue>>> expression,
           ChoiceList<TChoiceItem, TValue, TDisplay> choiceList,
           string name="ChoiceList")
            where TValue : IComparable
        {
            
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (choiceList == null) throw (new ArgumentNullException("choiceList"));
            string themeName = ThemedControlsStrings.GetTheme();
            ViewDataDictionary<dynamic> dataDictionary =
                       new ViewDataDictionary<dynamic>(htmlHelper.ViewData.Model);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            dataDictionary["ThemeParams"] =
               new ChoiceListDescription
               {
                   Expression =expression,
                   ChoiceList = choiceList,
                   HtmlHelper = htmlHelper
               };
            MvcHtmlString res;
            
                
            res = htmlHelper.Partial("Themes/" + themeName + "/" + name, dataDictionary);
            
            return res;
        }
        public static MvcHtmlString CheckBoxListFor<TModel, TChoiceItem, TValue, TDisplay>
            (
           this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, IEnumerable<TValue>>> expression,
           ChoiceList<TChoiceItem, TValue, TDisplay> choiceList,
           bool useTemplate = false,
           object template = null,
           object itemTemplate=null)
            where TValue : IComparable
        {
            IEnumerable<TValue> values = default(IEnumerable<TValue>);
            if (expression == null) throw (new ArgumentNullException("expression"));
            try
            {
                values = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch{}
            if (values == null) values = new List<TValue>();
            if (choiceList == null) throw (new ArgumentNullException("choiceList"));

            var fullPropertyPath =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));
            var propertyPath =
                  
                      ExpressionHelper.GetExpressionText(expression);
            CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay> displayModel =
                new CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>();
            displayModel.ImportFromModel(values, new object[] { choiceList });

            StringBuilder sb = new StringBuilder();
            
            sb.Append(BasicHtmlHelper.RenderDisplayInfo(htmlHelper,
                typeof(CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>),
                propertyPath));
               
            int index = 0;
            foreach (CheckBoxListItem<TValue> item in displayModel.Items)
            {
                sb.Append(htmlHelper.Hidden
                    (EnumerableHelper.CreateSubIndexName(BasicHtmlHelper.AddField(propertyPath, "$.Items"), index) + ".Code",
                    item.Code));
                index++;
            }
            if (useTemplate)
            {
                if (template == null && itemTemplate == null) template = typeof(CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>).Name;
                if (template != null)
                {
                    ViewDataDictionary<CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>> dataDictionary =
                        new ViewDataDictionary<CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>>(displayModel);
                    dataDictionary.TemplateInfo.HtmlFieldPrefix = (BasicHtmlHelper.AddField(fullPropertyPath, "$"));

                    sb.Append(new TemplateInvoker<CheckBoxList<IEnumerable<TValue>, TChoiceItem, TValue, TDisplay>>(template).Invoke<TModel>(htmlHelper, dataDictionary));
                }
                else
                {
                    int itemIndex =0;
                    foreach (CheckBoxListItem<TValue> item in displayModel.Items)
                    {
                        ViewDataDictionary<CheckBoxListItem<TValue>> dataDictionary =
                        new ViewDataDictionary<CheckBoxListItem<TValue>>(item);
                        dataDictionary.TemplateInfo.HtmlFieldPrefix = (BasicHtmlHelper.AddField(fullPropertyPath, string.Format("$.Items[{0}]", itemIndex)));
                        sb.Append(new TemplateInvoker<CheckBoxListItem<TValue>>(itemTemplate)
                        .Invoke(htmlHelper, dataDictionary));
                        itemIndex++;
                    }
                }


            }
            else
            {
                index = 0;
                foreach (CheckBoxListItem<TValue> item in displayModel.Items)
                {

                    sb.Append("<div>");
                    sb.Append(htmlHelper.CheckBox(
                        EnumerableHelper.CreateSubIndexName(BasicHtmlHelper.AddField(propertyPath, "$.Items"), index) + ".Selected",
                        item.Selected,
                        item.DisplayAttributes));
                    sb.Append("&nbsp;");
                    sb.Append(string.Format(CultureInfo.InvariantCulture,
                        "<span {0}>{1}</span>",
                        BasicHtmlHelper.GetAttributesString(item.LabelAttributes),
                        item.Label));
                    sb.Append("</div>");
                    index++;
                }
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    
    
    }
}

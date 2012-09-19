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
    public class MutualExclusionList<TChoiceItem, TValue, TDisplay> : IDisplayModel
        where TValue : IComparable, IConvertible
       
    {
        public MutualExclusionListItem<TValue>[] Items { get; set; }
        public object ExportToModel(params object[] context)
        {
            

            foreach (MutualExclusionListItem<TValue> x in Items)
            {
                if (x.Selected) return x.Value;

            }
            return default(TValue);
        }


        public void ImportFromModel(object model, params object[] context)
        {
            TValue input = default(TValue);
            if (model != null) input = (TValue)model;
            

            ChoiceList<TChoiceItem, TValue, TDisplay> choiceList = context[0] as ChoiceList<TChoiceItem, TValue, TDisplay>;

            List<MutualExclusionListItem<TValue>> items = new List<MutualExclusionListItem<TValue>>();
           

            foreach (TChoiceItem item in choiceList.Items)
            {
                MutualExclusionListItem<TValue> mutualExclusionListItem = new MutualExclusionListItem<TValue>();
                
                mutualExclusionListItem.Value = choiceList.ValueSelector(item);
                mutualExclusionListItem.Label = choiceList.DisplaySelector(item).ToString();

                if (choiceList.LabelAttributesSelector != null)
                    mutualExclusionListItem.LabelAttributes = choiceList.LabelAttributesSelector(item);
                else
                    mutualExclusionListItem.LabelAttributes = new { };
                if (choiceList.DisplayAttributesSelector != null)
                    mutualExclusionListItem.DisplayAttributes = choiceList.DisplayAttributesSelector(item);
                else
                    mutualExclusionListItem.DisplayAttributes = new { };
                items.Add(mutualExclusionListItem);

                if (((IComparable)input).CompareTo(mutualExclusionListItem.Value) == 0) mutualExclusionListItem.Selected = true;

            }
            Items = items.ToArray();
            

        }
    }

    public class MutualExclusionListItem<TValue>
    {
        public bool Selected { get; set; }
        public string Label { get; set; }
        internal TValue Value { get; set; }
        public object DisplayAttributes { get; set; }
        public object LabelAttributes { get; set; }

    }

    

    public static class MutualExclusionListHelper
    {
        public static MvcHtmlString MutualExclusionListFor<TModel, TValue, TChoiceItem, TDisplay>
            (
           this HtmlHelper<TModel> htmlHelper,
           Expression<Func<TModel, TValue>> expression,
           ChoiceList<TChoiceItem, TValue, TDisplay> choiceList,
           bool useTemplate = false,
           string templateName = null)
            where TValue : IComparable, IConvertible
        {
            

            if (expression == null) throw (new ArgumentNullException("expression"));
            if (choiceList == null) throw (new ArgumentNullException("choiceList"));
            TValue value = default(TValue);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            var fullPropertyPath =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));

            MutualExclusionList<TChoiceItem, TValue, TDisplay> displayModel =
                new MutualExclusionList<TChoiceItem, TValue, TDisplay>();
            displayModel.ImportFromModel(value, new object[] { choiceList });

            StringBuilder sb = new StringBuilder();
            sb.Append(htmlHelper.Hidden(fullPropertyPath + ".$$",
                    (typeof(MutualExclusionList<TChoiceItem, TValue, TDisplay>)).AssemblyQualifiedName).ToString());
            int index = 0;
            foreach (MutualExclusionListItem<TValue> item in displayModel.Items)
            {
                sb.Append(htmlHelper.Hidden
                    (EnumerableHelper.CreateSubIndexName(fullPropertyPath + ".Items", index) + ".Value",
                    item.Value));
                index++;
            }
            if (useTemplate)
            {
                if (templateName == null) templateName = typeof(MutualExclusionList<TChoiceItem, TValue, TDisplay>).Name;

                ViewDataDictionary<MutualExclusionList<TChoiceItem, TValue, TDisplay>> dataDictionary =
                    new ViewDataDictionary<MutualExclusionList<TChoiceItem, TValue, TDisplay>>(displayModel);
                dataDictionary.TemplateInfo.HtmlFieldPrefix = fullPropertyPath;

                sb.Append(htmlHelper.Partial(templateName, dataDictionary).ToString());

                
            }
            else
            {
                index=0;
                foreach (MutualExclusionListItem<TValue> item in displayModel.Items)
                {

                    sb.Append("<div>");
                    sb.Append(htmlHelper.RadioButton(
                        EnumerableHelper.CreateSubIndexName(fullPropertyPath + ".Items", index) + ".Selected",
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

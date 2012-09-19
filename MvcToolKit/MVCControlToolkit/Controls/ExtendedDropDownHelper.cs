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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MVCControlsToolkit.Core;
using System.Reflection;
using MVCControlsToolkit.Controls.Bindings;

namespace MVCControlsToolkit.Controls
{
    internal class ExtendedSelectListItem : SelectListItem
    {
        public string GroupKey { get; set; }
        public string GroupName { get; set; }
        public object Attributes { get; set; }
        public object GroupAttributes { get; set; }
    }
    public static class ChoiceListHelper
    {
        public static TDisplay DisplayValue<TItem, TValue, TDisplay>(
            IEnumerable<TItem> allValues,
            Expression<Func<TItem, TValue>> VSelector,
            Expression<Func<TItem, TDisplay>> DSelector,
            TValue value,
            TDisplay defaultRes)
        {
            if (VSelector == null) throw (new ArgumentNullException("VSelector"));
            if (DSelector == null) throw (new ArgumentNullException("DSelector"));

            PropertyInfo VProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(VSelector));
            PropertyInfo DProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(DSelector));
            foreach (TItem item in allValues)
            {
                if (Object.Equals(value, VProperty.GetValue(item, new object[0]))){
                    object res = DProperty.GetValue(item, new object[0]);
                    if (res != null) return (TDisplay)res;
                }
            }
            return defaultRes;
        }
        public static List<TDisplay> DisplayValues<TItem, TValue, TDisplay>(
            IEnumerable<TItem> allValues,
            Expression<Func<TItem, TValue>> VSelector,
            Expression<Func<TItem, TDisplay>> DSelector,
            List<TValue> value)
        {
            if (VSelector == null) throw (new ArgumentNullException("VSelector"));
            if (DSelector == null) throw (new ArgumentNullException("DSelector"));
            if (value == null) return new List<TDisplay>();
            HashSet<TValue> selecteds = new HashSet<TValue>(value);
            PropertyInfo VProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(VSelector));
            PropertyInfo DProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(DSelector));
            List<TDisplay> fres = new List<TDisplay>();
            foreach (TItem item in allValues)
            {
                if (selecteds.Contains((TValue)(VProperty.GetValue(item, new object[0]))))
                {
                    object res = DProperty.GetValue(item, new object[0]);
                    if (res != null) fres.Add((TDisplay)res);
                }
            }
            return fres;
        }
        public static List<KeyValuePair<TDisplay, TDisplayGroup>> DisplayValues<TItem, TValue, TDisplay, TDisplayGroup>(
            IEnumerable<TItem> allValues,
            Expression<Func<TItem, TValue>> VSelector,
            Expression<Func<TItem, TDisplay>> DSelector,
            Expression<Func<TItem, TDisplayGroup>> GDisplaySelector,
            List<TValue> value)
        {
            if (VSelector == null) throw (new ArgumentNullException("VSelector"));
            if (DSelector == null) throw (new ArgumentNullException("DSelector"));
            if (GDisplaySelector == null) throw (new ArgumentNullException("GDisplaySelector"));
            if (value == null) return new List<KeyValuePair<TDisplay, TDisplayGroup>>();
            HashSet<TValue> selecteds = new HashSet<TValue>(value);
            PropertyInfo VProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(VSelector));
            PropertyInfo DProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(DSelector));
            PropertyInfo GProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(GDisplaySelector));
            List<KeyValuePair<TDisplay, TDisplayGroup>> fres = new List<KeyValuePair<TDisplay, TDisplayGroup>>();
            foreach (TItem item in allValues)
            {
                if (selecteds.Contains((TValue)(VProperty.GetValue(item, new object[0]))))
                {
                    object res = DProperty.GetValue(item, new object[0]);
                    object gres = GProperty.GetValue(item, new object[0]);
                    if (res != null) {
                        if (gres != null)
                            fres.Add(new KeyValuePair<TDisplay, TDisplayGroup>((TDisplay)res, (TDisplayGroup)gres));
                        else
                            fres.Add(new KeyValuePair<TDisplay, TDisplayGroup>((TDisplay)res, default(TDisplayGroup)));
                    }
                }
            }
            return fres;
        }
        public static KeyValuePair<TDisplay, TDisplayGroup> DisplayValue<TItem, TValue, TDisplay, TDisplayGroup>(
            IEnumerable<TItem> allValues,
            Expression<Func<TItem, TValue>> VSelector,
            Expression<Func<TItem, TDisplay>> DSelector,
            Expression<Func<TItem, TDisplayGroup>> GDisplaySelector,
            TValue value,
            TDisplay defaultRes)
        {
            if (VSelector == null) throw (new ArgumentNullException("VSelector"));
            if (DSelector == null) throw (new ArgumentNullException("DSelector"));
            if (GDisplaySelector == null) throw (new ArgumentNullException("GDisplaySelector"));

            PropertyInfo VProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(VSelector));
            PropertyInfo DProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(DSelector));
            PropertyInfo GProperty = typeof(TItem).GetProperty(ExpressionHelper.GetExpressionText(GDisplaySelector));

            foreach (TItem item in allValues)
            {
                if (Object.Equals(value, VProperty.GetValue(item, new object[0])))
                {
                    object res = DProperty.GetValue(item, new object[0]);
                    object gres = GProperty.GetValue(item, new object[0]);
                    if (res != null)
                    {
                        if (gres != null)
                            return new KeyValuePair<TDisplay, TDisplayGroup>((TDisplay)res, (TDisplayGroup)gres);
                        else
                            return new KeyValuePair<TDisplay, TDisplayGroup>((TDisplay)res, default(TDisplayGroup));
                    }
                }
            }
            return new KeyValuePair<TDisplay, TDisplayGroup>(defaultRes, default(TDisplayGroup));
        }
        public static ChoiceList<TItem, TValue, TDisplay>
            Create<TItem, TValue, TDisplay>(IEnumerable<TItem> items,
            Expression<Func<TItem, TValue>> valueSelector,
            Expression<Func<TItem, TDisplay>> displaySelector,
            Func<TItem, object> labelAttributesSelector = null,
            Func<TItem, object> valueAttributesSelector = null,
            bool usePrompt = true,
            string overridePrompt = null)
        {
            return new ChoiceList<TItem, TValue, TDisplay>
                (items, valueSelector, displaySelector, 
                valueAttributesSelector, labelAttributesSelector,
                usePrompt, overridePrompt);
        }
        public static ChoiceList<TItem, TValue, TDisplay>
            CreateChoiceList<VM, TItem, TValue, TDisplay>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<TItem>>> expression,
            Expression<Func<TItem, TValue>> valueSelector,
            Expression<Func<TItem, TDisplay>> displaySelector,
            Func<TItem, object> labelAttributesSelector = null,
            Func<TItem, object> valueAttributesSelector = null,
            bool usePrompt = true,
            string overridePrompt = null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IEnumerable<TItem> items = null;
            try
            {
                items = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            ChoiceList<TItem, TValue, TDisplay> res = new ChoiceList<TItem, TValue, TDisplay>
                (items, valueSelector, displaySelector,
                valueAttributesSelector, labelAttributesSelector,
                usePrompt, overridePrompt);
            res.origin = expression;
            res.isInClientBlock = htmlHelper.ClientBindings() != null;
            return res;
        }
        public static ChoiceList<TItem, TValue, TDisplay>
            CreateGrouped<TItem, TValue, TDisplay, TGroup, TDisplayGroup>(IEnumerable<TItem> items,
            Expression<Func<TItem, TValue>> valueSelector,
            Expression<Func<TItem, TDisplay>> displaySelector,
            Func<TItem, TGroup> groupValueSelector,
            Func<TItem, TDisplayGroup> groupDisplaySelector,
            Func<TItem, object> groupAttributesSelector = null,
            Func<TItem, object> labelAttributesSelector = null,
            Func<TItem, object> valueAttributesSelector = null,
            bool usePrompt = true,
            string overridePrompt=null)
        {
            return new GroupedChoiceList<TItem, TValue, TDisplay, TGroup, TDisplayGroup>
                (items, valueSelector, displaySelector,
                 groupValueSelector, groupDisplaySelector,
                groupAttributesSelector, valueAttributesSelector, labelAttributesSelector, 
                usePrompt, overridePrompt);
        }
        public static ChoiceList<TItem, TValue, TDisplay>
            CreateGroupedChoiceList<VM, TItem, TValue, TDisplay, TGroup, TDisplayGroup>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<TItem>>> expression,
            Expression<Func<TItem, TValue>> valueSelector,
            Expression<Func<TItem, TDisplay>> displaySelector,
            Func<TItem, TGroup> groupValueSelector,
            Func<TItem, TDisplayGroup> groupDisplaySelector,
            Func<TItem, object> groupAttributesSelector = null,
            Func<TItem, object> labelAttributesSelector = null,
            Func<TItem, object> valueAttributesSelector = null,
            bool usePrompt = true,
            string overridePrompt = null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IEnumerable<TItem> items = null;
            try
            {
                items = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            GroupedChoiceList<TItem, TValue, TDisplay, TGroup, TDisplayGroup> res = new GroupedChoiceList<TItem, TValue, TDisplay, TGroup, TDisplayGroup>
                (items, valueSelector, displaySelector,
                 groupValueSelector, groupDisplaySelector,
                groupAttributesSelector, valueAttributesSelector, labelAttributesSelector,
                usePrompt, overridePrompt);
            res.origin = expression;
            res.isInClientBlock = htmlHelper.ClientBindings() != null;
            return res;
        }
    }
    public static class ExtendedDropDownHelper
    {
        private static MvcHtmlString ClientDropDownListForBase<VM, M, TItem, TDisplay, TValue>(
             HtmlHelper<VM> htmlHelper, Expression<Func<VM, M>> expression, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items, bool isList, ModelMetadata metaData)
        {
            MVCControlsToolkit.Controls.Bindings.IBindingsBuilder<VM> bindings = htmlHelper.ClientBindings();
            if (htmlAttributes == null)
            {
                htmlAttributes = new Dictionary<string, object>();
            }
            Expression<Func<VM, IEnumerable<TItem>>> itemsExpression = items.origin as Expression<Func<VM, IEnumerable<TItem>>>;
            string prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            string caption = null;
            if (!isList){
                caption=items.OverridePrompt;
                if (caption == null) caption = metaData.Watermark; 
            }
            if (typeof(TItem) == typeof(TValue))
                bindings.Options<VM, IEnumerable<TItem>>(itemsExpression, caption);
            else
                bindings.Options<VM, TItem, TValue, TDisplay>(itemsExpression, items._evalueSelector, items._edisplaySelector, caption);
            
            if (isList)
            {
                htmlAttributes["multiple"] = "multiple";
                bindings.SelectedOptions(expression);
            }
            else
            {
                bindings.Value(expression);
            }
            string prevBind = null;
            if (htmlAttributes.ContainsKey("data-bind"))
            {
                prevBind = htmlAttributes["data-bind"] as string;
            }
            if (string.IsNullOrWhiteSpace(prevBind))
            {
                htmlAttributes["data-bind"] = bindings.Get();
            }
            else
                htmlAttributes["data-bind"] = prevBind+", "+bindings.Get();
            htmlAttributes["data-nobinding"] = true;
            return MvcHtmlString.Create(string.Format("<select id = '{0}' name = '{1}' {2} ></select>", BasicHtmlHelper.IdFromName(prefix), prefix, BasicHtmlHelper.GetAttributesString(htmlAttributes)));
        }

        internal static string ListItemToOption(ExtendedSelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };

            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            if (item.Attributes != null)
            {
                IDictionary<string, object> attrs = item.Attributes as IDictionary<string, object>;
                if (attrs == null) attrs = new RouteValueDictionary(item.Attributes);
                builder.MergeAttributes<string, object>(attrs);
            }

            return builder.ToString(TagRenderMode.Normal);
        }
        internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType)
        {
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null);
                }
            }
            return null;
        }

        private static List<ExtendedSelectListItem> prepareItems<TItem, TDisplay, TValue>(ChoiceList<TItem, TDisplay, TValue> choices, bool isDropDown, ModelMetadata model)
        {
            List<ExtendedSelectListItem> res = new List<ExtendedSelectListItem>();
            if (choices.isInClientBlock) return res;
            if (isDropDown && (choices.UsePrompt || choices.OverridePrompt != null))
            {
                string prompt = choices.OverridePrompt;
                if (prompt == null)
                {
                    prompt = model.Watermark;
                }
                if (prompt != null)
                {
                    res.Add(new ExtendedSelectListItem
                    {
                        Value = string.Empty,
                        Text = prompt,
                        Attributes =
                            choices.LabelAttributesSelector == null ?
                                null : choices.LabelAttributesSelector(default(TItem))
                    });
                }
            }
            foreach(TItem item in choices.Items)
            {
                res.Add(new ExtendedSelectListItem
                    {
                        Attributes =
                            choices.LabelAttributesSelector == null ?
                                null : choices.LabelAttributesSelector(item),
                        Value = Convert.ToString(choices.ValueSelector(item), CultureInfo.CurrentCulture),
                        Text = Convert.ToString(choices.DisplaySelector(item), CultureInfo.CurrentCulture),
                        GroupKey = choices is IGroupedChoiceList ? Convert.ToString(((IGroupedChoiceList)choices).GroupValueSelector(item), CultureInfo.CurrentCulture) : null,
                        GroupName = choices is IGroupedChoiceList ? Convert.ToString(((IGroupedChoiceList)choices).GroupDisplaySelector(item), CultureInfo.CurrentCulture):null,
                        GroupAttributes = choices is IGroupedChoiceList && ((IGroupedChoiceList)choices).GroupAttributesSelector != null ? 
                            ((IGroupedChoiceList)choices).GroupAttributesSelector(item) : null
                    });
                
            }
            return res;
        }
        

        internal static MvcHtmlString DropDownbase<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, string name, object value, List<ExtendedSelectListItem> items, IDictionary<string, object> htmlAttributes, bool allowsMultipleValues, bool useGroups, ModelMetadata metaData=null)
        {
            string partialName = name;
            if (name == null) throw new ArgumentNullException("name");
            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            
            if (items == null) throw new ArgumentNullException("items");
            object defaultValue = (allowsMultipleValues) ? htmlHelper.GetModelStateValue(name, typeof(string[])) : htmlHelper.GetModelStateValue(name, typeof(string));
            if (defaultValue == null)
            {
                defaultValue = value;
            }
            if (defaultValue != null)
            {
                IEnumerable defaultValues = (allowsMultipleValues) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = (from object currValue in defaultValues select Convert.ToString(currValue, CultureInfo.CurrentCulture)).ToList();
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

                foreach (ExtendedSelectListItem item in items)
                {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                }
            }
            StringBuilder listItemBuilder = new StringBuilder();

            if (useGroups)
            {
                foreach (var group in items.GroupBy(i => i.GroupKey))
                {
                    var groupProps = group.Select(it => new { Name = it.GroupName, Attrs = it.GroupAttributes }).FirstOrDefault();
                    IDictionary<string, object> dictionary = groupProps.Attrs as IDictionary<string, object>;
                    if (group.Key != null && groupProps.Name != null)
                    {
                        if (dictionary != null)
                        {
                            listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\" {2}>",
                                htmlHelper.Encode(groupProps.Name),
                                htmlHelper.Encode(group.Key),
                                BasicHtmlHelper.GetAttributesString(dictionary)));
                        }
                        else
                        {
                            listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\" {2}>",
                                htmlHelper.Encode(groupProps.Name),
                                htmlHelper.Encode(group.Key),
                                BasicHtmlHelper.GetAttributesString(groupProps.Attrs)));
                        }
                    }
                    foreach (ExtendedSelectListItem item in group)
                    {
                        listItemBuilder.AppendLine(ListItemToOption(item));
                    }
                    if (group.Key != null && groupProps.Name != null) listItemBuilder.AppendLine("</optgroup>");
                }
            }
            else
            {
                foreach (ExtendedSelectListItem item in items)
                {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };

            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", name, true /* replaceExisting */);
            tagBuilder.GenerateId(name);
            if (allowsMultipleValues)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }
            tagBuilder.MergeAttributes(MvcEnvironment.GetUnobtrusiveValidation(htmlHelper, partialName, metaData));
            return MvcHtmlString.Create(tagBuilder.ToString());
        }
        public static MvcHtmlString DropDownList<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, string name, TValue value, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, true, ModelMetadata.FromStringExpression(name, htmlHelper.ViewData)), null, false, items is IGroupedChoiceList);
        }
        public static MvcHtmlString DropDownList<VM, TItem, TDisplay, TValue>(
           this HtmlHelper<VM> htmlHelper, string name, IEnumerable<TValue> value, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, false, null), null, true, items is IGroupedChoiceList);
        }
        public static MvcHtmlString DropDownList<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, string name, TValue value, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, true, ModelMetadata.FromStringExpression(name, htmlHelper.ViewData)), htmlAttributes, false, items is IGroupedChoiceList);
        }
        public static MvcHtmlString DropDownList<VM, TItem, TDisplay, TValue>(
           this HtmlHelper<VM> htmlHelper, string name, IEnumerable<TValue> value, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, false, null), htmlAttributes, true, items is IGroupedChoiceList);
        }
        public static MvcHtmlString DropDownList<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, string name, TValue value, object htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, true, ModelMetadata.FromStringExpression(name, htmlHelper.ViewData)), new RouteValueDictionary(htmlAttributes), false, items is IGroupedChoiceList);
        }
        public static MvcHtmlString DropDownList<VM, TItem, TDisplay, TValue>(
           this HtmlHelper<VM> htmlHelper, string name, IEnumerable<TValue> value, object htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, false, null), new RouteValueDictionary(htmlAttributes), true, items is IGroupedChoiceList);
        }

        private static MvcHtmlString DropDownListForBase<VM, TItem, TDisplay, TValue>(
             HtmlHelper<VM> htmlHelper, Expression<Func<VM, TValue>> expression, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (items.isInClientBlock)
            {
                return ClientDropDownListForBase<VM, TValue, TItem, TDisplay, TValue>(
                    htmlHelper,
                    expression,
                    htmlAttributes,
                    items,
                    false,
                    metaData);
            }
            string name = ExpressionHelper.GetExpressionText(expression);
            TValue value = default(TValue);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, true, metaData), htmlAttributes, false, items is IGroupedChoiceList, metaData);
        }
        private static MvcHtmlString DropDownListForBase<VM, TItem, TDisplay, TValue>(
            HtmlHelper<VM> htmlHelper, Expression<Func<VM, IEnumerable<TValue>>> expression, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            if (items.isInClientBlock)
            {
                return ClientDropDownListForBase<VM, IEnumerable<TValue>, TItem, TDisplay, TValue>(
                    htmlHelper,
                    expression,
                    htmlAttributes,
                    items,
                    true,
                    null);
            }
            string name = ExpressionHelper.GetExpressionText(expression);
            IEnumerable<TValue> value = null;
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            return DropDownbase<VM, TItem, TDisplay, TValue>(
             htmlHelper, name, value, prepareItems(items, false, null), htmlAttributes, true, items is IGroupedChoiceList);
        }


        public static MvcHtmlString DropDownListFor<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, Expression<Func<VM, TValue>> expression, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownListForBase(htmlHelper, expression, htmlAttributes, items); 
        }
        public static MvcHtmlString DropDownListFor<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, Expression<Func<VM, IEnumerable<TValue>>> expression, IDictionary<string, object> htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownListForBase(htmlHelper, expression, htmlAttributes, items);
        }
        public static MvcHtmlString DropDownListFor<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, Expression<Func<VM, TValue>> expression, object htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownListForBase<VM, TItem, TDisplay, TValue>(
                htmlHelper, expression, new RouteValueDictionary(htmlAttributes), items);
        }
        public static MvcHtmlString DropDownListFor<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, Expression<Func<VM, IEnumerable<TValue>>> expression, object htmlAttributes, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownListForBase<VM, TItem, TDisplay, TValue>(
                htmlHelper, expression, new RouteValueDictionary(htmlAttributes), items);
        }
        public static MvcHtmlString DropDownListFor<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, Expression<Func<VM, TValue>> expression, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownListForBase<VM, TItem, TDisplay, TValue>(
                htmlHelper, expression, null, items);
        }
        public static MvcHtmlString DropDownListFor<VM, TItem, TDisplay, TValue>(
            this HtmlHelper<VM> htmlHelper, Expression<Func<VM, IEnumerable<TValue>>> expression, ChoiceList<TItem, TValue, TDisplay> items)
        {
            return DropDownListForBase<VM, TItem, TDisplay, TValue>(
                htmlHelper, expression, null, items);
        }
    }
    
}

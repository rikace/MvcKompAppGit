using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using System.Globalization;
using System.Web.Routing;

namespace MVCControlsToolkit.Controls
{
    public static class RangeHelpers
    {
        
        public static MvcHtmlString Range<VM>(
                this HtmlHelper<VM> htmlHelper,
                string name,
                object value,
                object htmlAttributes
            )
        {
            return htmlHelper.MinMaxBase("range", name, value, null, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString InputNumber<VM>(
                this HtmlHelper<VM> htmlHelper,
                string name,
                object value,
                object htmlAttributes
            )
        {
            return htmlHelper.MinMaxBase("number", name, value, null, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString Range<VM>(
                this HtmlHelper<VM> htmlHelper,
                string name,
                object value,
                IDictionary<string, object> htmlAttributes
            )
        {
            return htmlHelper.MinMaxBase("range", name, value, null, htmlAttributes);
        }
        public static MvcHtmlString InputNumber<VM>(
                this HtmlHelper<VM> htmlHelper,
                string name,
                object value,
            IDictionary<string, object> htmlAttributes
            )
        {
            return htmlHelper.MinMaxBase("number", name, value, null, htmlAttributes);
        }
        public static MvcHtmlString Range<VM>(
                this HtmlHelper<VM> htmlHelper,
                string name,
                object value
            )
        {
            return htmlHelper.MinMaxBase("range", name, value, null, null);
        }
        public static MvcHtmlString InputNumber<VM>(
                this HtmlHelper<VM> htmlHelper,
                string name,
                object value
            )
        {
            return htmlHelper.MinMaxBase("number", name, value, null, null);
        }
        public static MvcHtmlString RangeFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression)
        {
            return htmlHelper.MinMaxBaseFor("range", expression, null);
        }
        public static MvcHtmlString InputNumberFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression)
        {
            return htmlHelper.MinMaxBaseFor("number", expression, null);
        }
        public static MvcHtmlString RangeFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.MinMaxBaseFor("range", expression, htmlAttributes);
        }
        public static MvcHtmlString InputNumberFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.MinMaxBaseFor("number", expression, htmlAttributes);
        }
        public static MvcHtmlString RangeFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression,
            object htmlAttributes)
        {
            return htmlHelper.MinMaxBaseFor("range", expression, new RouteValueDictionary(htmlAttributes));
        }
        public static MvcHtmlString InputNumberFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression,
            object htmlAttributes)
        {
            return htmlHelper.MinMaxBaseFor("number", expression, new RouteValueDictionary(htmlAttributes));
        }
        internal static MvcHtmlString MinMaxBaseFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            string type,
            Expression<Func<VM, T>> expression,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            T value = default(T);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            return htmlHelper.MinMaxBase(
                type,
                ExpressionHelper.GetExpressionText(expression),
                value,
                ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
                htmlAttributes);
        }
        internal static MvcHtmlString MinMaxBase<VM>(
            this HtmlHelper<VM> htmlHelper,
            string type,
            string name,
            object value,
            ModelMetadata metaData = null,
            IDictionary<string, object> htmlAttributes = null)
        {
            string partialName = name;
            if (name == null) throw new ArgumentNullException("name");
            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", name, true /* replaceExisting */);
            tagBuilder.MergeAttribute("type", type, true /* replaceExisting */);
            tagBuilder.MergeAttribute("value", Convert.ToString(value), true /* replaceExisting */);
            tagBuilder.GenerateId(name);
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }
            IDictionary<String, object> validation = MvcEnvironment.GetUnobtrusiveValidation(htmlHelper, name, metaData);
            if (validation != null)
            {
                if (validation.ContainsKey("data-val-dynamicrange-min"))
                    tagBuilder.MergeAttribute("min", Convert.ToString(validation["data-val-dynamicrange-min"]), false);
                if (validation.ContainsKey("data-val-dynamicrange-max"))
                    tagBuilder.MergeAttribute("max", Convert.ToString(validation["data-val-dynamicrange-max"]), false);
            }
            return MvcHtmlString.Create(tagBuilder.ToString());
        }
    }
}

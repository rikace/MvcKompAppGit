using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using System.Globalization;

namespace MVCControlsToolkit.Controls
{
     
    public static class GenericInputHelpers
    {
        private static string GetInputTypeString(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Text: return "text";
                case InputType.Password: return "password";
                case InputType.Radio: return "radio";
                case InputType.CheckBox: return "checkbox";
                default: return "hidden";
            }
        }
        public static MvcHtmlString GenericInput<VM>(
            this HtmlHelper<VM> htmlHelper,
            InputType inputType,
            string name,
            object value=null)
        {
            if (name == null) throw (new ArgumentNullException("name"));
            name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      name);
            string type = GetInputTypeString(inputType);
            return MvcHtmlString.Create(
                string.Format("<input type='{3}' name='{0}' id='{1}' value='{2}' />",
                name, BasicHtmlHelper.IdFromName(name),
                htmlHelper.Encode(Convert.ToString(value, CultureInfo.InvariantCulture)),
                type));

        }
        public static MvcHtmlString GenericInput<VM>(
            this HtmlHelper<VM> htmlHelper,
            InputType inputType,
            string name,
            object value,
            object htmlAttributes)
        {
            if (name == null) throw (new ArgumentNullException("name"));
            name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      name);
            string type = GetInputTypeString(inputType);
            return MvcHtmlString.Create(
                string.Format("<input type='{4}' name='{0}' id='{1}' value='{2}'  {3}/>",
                name, BasicHtmlHelper.IdFromName(name),
                htmlHelper.Encode(Convert.ToString(value, CultureInfo.InvariantCulture)),
                BasicHtmlHelper.GetAttributesString(htmlAttributes), type));

        }
        public static MvcHtmlString GenericInput<VM>(
            this HtmlHelper<VM> htmlHelper,
            InputType inputType,
            string name,
            object value,
            IDictionary<string, object> htmlAttributes)
        {
            if (name == null) throw (new ArgumentNullException("name"));
            name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      name);
            string type = GetInputTypeString(inputType);
            return MvcHtmlString.Create(
                string.Format("<input type='{4}' name='{0}' id='{1}' value='{2}'  {3}/>",
                name, BasicHtmlHelper.IdFromName(name),
                htmlHelper.Encode(Convert.ToString(value, CultureInfo.InvariantCulture)),
                BasicHtmlHelper.GetAttributesString(htmlAttributes), type));
        }
        public static MvcHtmlString GenericInputFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            InputType inputType,
            Expression<Func<VM, T>> expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));

            var name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));
            string type = GetInputTypeString(inputType);
            T value = default(T);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }

            return MvcHtmlString.Create(
                string.Format("<input type='{3}' name='{0}' id='{1}' value='{2}' />",
                name, BasicHtmlHelper.IdFromName(name),
                htmlHelper.Encode(Convert.ToString(value, CultureInfo.InvariantCulture)),
                type));

        }
        public static MvcHtmlString GenericInputFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            InputType inputType,
            Expression<Func<VM, T>> expression,
            object htmlAttributes)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));

            var name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));
            string type = GetInputTypeString(inputType);
            T value = default(T);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            
            return MvcHtmlString.Create(
                string.Format("<input type='{4}' name='{0}' id='{1}' value='{2}'  {3}/>",
                name, BasicHtmlHelper.IdFromName(name),
                htmlHelper.Encode(Convert.ToString(value, CultureInfo.InvariantCulture)),
                BasicHtmlHelper.GetAttributesString(htmlAttributes), type));

        }
        public static MvcHtmlString GenericInputFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            InputType inputType,
            Expression<Func<VM, T>> expression,
            IDictionary<string, object> htmlAttributes )
        {
            if (expression == null) throw (new ArgumentNullException("expression"));

            var name =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));
            string type = GetInputTypeString(inputType);
            T value = default(T);
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            
            return MvcHtmlString.Create(
                string.Format("<input type='{4}' name='{0}' id='{1}' value='{2}'  {3}/>",
                name, BasicHtmlHelper.IdFromName(name),
                htmlHelper.Encode(Convert.ToString(value, CultureInfo.InvariantCulture)),
                BasicHtmlHelper.GetAttributesString(htmlAttributes), type));

        }
    }
}

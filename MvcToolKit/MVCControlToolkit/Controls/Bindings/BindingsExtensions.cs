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
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls;

namespace MVCControlsToolkit.Controls.Bindings
{
    
    public static class BindingsExtensions
    {
        private static string textScript =
        "text: MvcControlsToolkit_FormatDisplay({0}(), &quot;{1}&quot;, {2}, &quot;{3}&quot;, &quot;{4}&quot;, &quot;{5}&quot;)";
        public static IBindingsBuilder<T> Visible<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("visible", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Text<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (format == null)
            {
                string clientFormat;
                string prefix;
                string postfix;
                string nullString;
                int clientType=CoreHTMLHelpers.
                    GetClientInfo(expression, out clientFormat, out prefix, out postfix, out nullString);
                bindingsBuilder.Add(
                    string.Format(textScript, 
                        bindingsBuilder.GetFullBindingName(expression),
                        clientFormat,
                        clientType,
                        prefix,
                        postfix,
                        nullString)
                    );
                return bindingsBuilder;
            }
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("text", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Html<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("html", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Enable<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("enable", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Disable<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("disable", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Value<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string clientFormat;
            string prefix;
            string postfix;
            string nullString;
            int clientType = CoreHTMLHelpers.
                GetClientInfo(expression, out clientFormat, out prefix, out postfix, out nullString);
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("value", expression, null, null)
                );
            bindingsBuilder.Add("valueType: " + clientType);
            if (clientFormat.Length > 0)
                bindingsBuilder.Add("formatString: &quot;" + clientFormat + "&quot;");
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Checked<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("checked", expression, format, otherExpressions)
                );
            bindingsBuilder.Add("valueType: " + CoreHTMLHelpers.GetClientType(typeof(F)));
            return bindingsBuilder;
        }
        
        public static IBindingsBuilder<T> Options<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string caption=null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("options", expression, null, null)
                );

            int clientType = CoreHTMLHelpers.GetClientType(typeof(F));
            bindingsBuilder.Add("valueType: " + clientType);
            
            if (!string.IsNullOrWhiteSpace(caption))
                bindingsBuilder.Add("optionsCaption: &quot;" + caption + "&quot;");
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Options<T, F, IF, IFT>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, IEnumerable<F>>> expression,
            Expression<Func<F, IF>> valueField,
            Expression<Func<F, IFT>> textField,
            string caption=null,
            string valueFieldFormat=null,
            string textFieldFormat=null)
  
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, IEnumerable<F>>("options", expression, null, null)
                );
            if (valueFieldFormat == null)
                bindingsBuilder.Add("optionsValue: &quot;" + ExpressionHelper.GetExpressionText(valueField) + "&quot;");
            else
                bindingsBuilder.Add(
                   baseStandardString<F, IF>("optionsValue", valueField, valueFieldFormat, null)
                   );
            string clientFormat;
            string prefix;
            string postfix;
            string nullString;
            int clientType = CoreHTMLHelpers.
                GetClientInfo(valueField, out clientFormat, out prefix, out postfix, out nullString);
            bindingsBuilder.Add("valueType: " + clientType);
            if (clientFormat.Length > 0)
                bindingsBuilder.Add("formatString: &quot;" + clientFormat + "&quot;");
            if (textFieldFormat == null)
                bindingsBuilder.Add("optionsText: &quot;" + ExpressionHelper.GetExpressionText(textField) + "&quot;");
            else
                bindingsBuilder.Add(
                   baseStandardString<F, IFT>("optionsText", textField, textFieldFormat, null)
                   );
            clientType = CoreHTMLHelpers.
                GetClientInfo(textField, out clientFormat, out prefix, out postfix, out nullString);
            bindingsBuilder.Add("textType: " + clientType);
            if (clientFormat.Length > 0)
                bindingsBuilder.Add("textFormatString: &quot;" + clientFormat + "&quot;");
            bindingsBuilder.Add("textPrefix: &quot;" + prefix + "&quot;");
            bindingsBuilder.Add("textPostfix: &quot;" + postfix + "&quot;");
            bindingsBuilder.Add("textNullString: &quot;" + nullString + "&quot;");
            if (!string.IsNullOrWhiteSpace(caption))
                bindingsBuilder.Add("optionsCaption: &quot;" + caption + "&quot;");
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> SelectedOptions<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("selectedOptions", expression, format, otherExpressions)
                );
            bindingsBuilder.Add("valueType: " + CoreHTMLHelpers.GetClientType(typeof(F)));
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Template<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("template", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Template<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            string templateName,
            Expression<Func<T, F>> expression,
            string afterAdd=null,
            string beforeRemove=null,
            string afterRender=null,
            object templateOptions=null,
            string prefix=null,
            bool applyClientValidation=true,
            bool fastNoJavaScript=false,
            string afterAllRender=null,
            string templateEngine=null)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (string.IsNullOrWhiteSpace(templateName)) throw (new ArgumentNullException("templateName"));
            string format = null;
            string actualPrefix = BasicHtmlHelper.AddField(
                    bindingsBuilder.ModelPrefix,
                    ExpressionHelper.GetExpressionText(expression));
            if (prefix == null)
            {
                prefix = actualPrefix;
            }
            StringBuilder sb = new StringBuilder();
            if (templateName[0] != '@')
            {
                sb.Append("template: { name: &quot;");
                sb.Append(templateName);
                if (typeof(IEnumerable).IsAssignableFrom(typeof(F)))
                    sb.Append("&quot;, foreach: ");
                else
                    sb.Append("&quot;, data: ");
            }
            else
            {
                templateName = templateName.Substring(1);
                sb.Append("template: { name: ");
                sb.Append(templateName);
                if (typeof(IEnumerable).IsAssignableFrom(typeof(F)))
                    sb.Append(", foreach: ");
                else
                    sb.Append(", data: ");
            }
            sb.Append(bindingsBuilder.GetFullBindingName(expression));
            
            if (afterRender != null)
            {
                sb.Append(", afterRender: ");
                sb.Append(afterRender);
 
            }
            if (afterAdd != null)
            {
                sb.Append(", afterAdd: ");
                sb.Append(afterAdd);
                   
            }
            if (beforeRemove != null)
            {
                sb.Append(", beforeRemove: ");
                sb.Append(beforeRemove);
            }
            if (afterAllRender != null)
            {
                sb.Append(", afterAllRender: ");
                sb.Append(afterAllRender);
            }
            if (templateEngine != null)
            {
                sb.Append(", templateEngine: ko.");
                sb.Append(templateEngine);
                sb.Append(".instance");
            }
            var additionalOptions = new
            {
                ModelPrefix = "&quot;" + prefix + "&quot;",
                ModelId = "&quot;" + BasicHtmlHelper.IdFromName(prefix) + "&quot;",
                ItemPrefix = "&quot;&quot;",
                templateSymbol = "&quot;" + ClientTemplateHelper.templateSymbol + "0&quot;"
            };
            sb.Append(", templateOptions: {");
            if (templateOptions != null)
            {
                sb.Append(BasicHtmlHelper.TranslateAnonymous(templateOptions));
                sb.Append(", ");
            }
            sb.Append(BasicHtmlHelper.TranslateAnonymous(additionalOptions));
            
            sb.Append(" }");
            sb.Append(", processingOptions: {");
            if (bindingsBuilder.ValidationType == "UnobtrusiveClient")
            {
                sb.Append("unobtrusiveClient: true");
                if(bindingsBuilder.GetHelper().ViewData["_TemplateLevel_"] == null) bindingsBuilder.AddServerErrors(actualPrefix);
            }
            else sb.Append("unobtrusiveClient: false");
            sb.Append(fastNoJavaScript ? ", fastNoJavaScript: true" : ", fastNoJavaScript: false");
            sb.Append(applyClientValidation ? ", applyClientValidation: true" : ", applyClientValidation: false");
            sb.Append(" }");
            sb.Append(" }");
            format = sb.ToString();
            
            
            bindingsBuilder.Add(
                format
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> UniqueName<T>(this IBindingsBuilder<T> bindingsBuilder)
            where T : class
        {
            bindingsBuilder.Add(
                "uniqueName: true"
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Click<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("click", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> HasFocus<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("hasfocus", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }
        public static IBindingsBuilder<T> Submit<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            Expression<Func<T, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
            where T : class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            bindingsBuilder.Add(
                bindingsBuilder.standardString<T, F>("submit", expression, format, otherExpressions)
                );
            return bindingsBuilder;
        }

        internal static string standardString<T, F>(
            this IBindingsBuilder<T> bindingsBuilder,
            string bindingName,
            Expression<Func<T, F>> expression,
            string format,
            params LambdaExpression[] otherExpressions)

        {
            if (format == null)
            {
                return bindingName + ": " + bindingsBuilder.GetFullBindingName(expression);
            }
            else if (otherExpressions == null || otherExpressions.Length == 0)
            {
                return bindingName + ": " + string.Format(format, bindingsBuilder.GetFullBindingName(expression) + "()");
            }
            else
            {
                string[] args = new string[otherExpressions.Length + 1];
                args[0] = bindingsBuilder.GetFullBindingName(expression) + "()";
                int index = 1;
                foreach (LambdaExpression currExpression in otherExpressions)
                {
                    args[index] = bindingsBuilder.GetFullBindingName(currExpression) + "()";
                    index++;
                }
                return bindingName + ": " + string.Format(format, args);
            }
        }
        internal static string baseStandardString<T, F>(
            string bindingName,
            Expression<Func<T, F>> expression,
            string format,
            params LambdaExpression[] otherExpressions)
        {
            if (format == null)
            {
                return bindingName + ": " + ExpressionHelper.GetExpressionText(expression);
            }
            else if (otherExpressions == null || otherExpressions.Length == 0)
            {
                return bindingName + ": " + string.Format(format, ExpressionHelper.GetExpressionText(expression) + "()");
            }
            else
            {
                string[] args = new string[otherExpressions.Length + 1];
                args[0] = ExpressionHelper.GetExpressionText(expression) +"()";
                int index = 1;
                foreach (LambdaExpression currExpression in otherExpressions)
                {
                    args[index] = ExpressionHelper.GetExpressionText(expression) +"()";
                    index++;
                }
                return bindingName + ": " + string.Format(format, args);
            }
        }

    }
}

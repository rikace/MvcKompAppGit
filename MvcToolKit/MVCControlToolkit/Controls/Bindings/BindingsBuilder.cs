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
using MVCControlsToolkit.Controls;

namespace MVCControlsToolkit.Controls.Bindings
{
    public class BindingsBuilder<M> : IBindingsBuilder<M>
    {
        private static string methodScript=
            @"
            <script language='javascript' type='text/javascript'>
                {0}.{1} = {2};     
            </script>
            ";
        private StringBuilder sb;
        private StringBuilder cssStringBuilder;
        private StringBuilder styleStringBuilder;
        private StringBuilder attrStringBuilder;
        private StringBuilder eventStringBuilder;
        private System.IO.TextWriter writer;
        private string modelName;
        private string modelPrefix;
        private string bindingPrefix="";
        private string validationType;
        private string hiddenId;
        private StringBuilder errorsWriter;
        private HtmlHelper htmlHelper;
        
        public string ModelName
        {
            get{
            return modelName;
            }
        }
        public string BindingPrefix
        {
            get
            {
                return bindingPrefix;
            }
        }
        public BindingsBuilder(System.IO.TextWriter writer, string modelName, string modelPrefix, string validationType, string hiddenId, HtmlHelper htmlHelper, string bindingPrefix="")
        {
            
            this.writer = writer;
            this.modelName = modelName;
            this.modelPrefix = modelPrefix;
            this.validationType = validationType;
            this.hiddenId = hiddenId;
            this.htmlHelper = htmlHelper;
            this.bindingPrefix = bindingPrefix;
        }
        public void SetHelper(HtmlHelper htmlHelper)
        {
            if (this.htmlHelper != htmlHelper)
            {
                this.htmlHelper = htmlHelper;
                switch (MvcEnvironment.Validation(htmlHelper))
                {
                    case MVCControlsToolkit.Core.ValidationType.StandardClient: validationType = "StandardClient"; break;
                    case MVCControlsToolkit.Core.ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                    default: validationType = "Server"; break;
                }
            }
        }
        public HtmlHelper GetHelper()
        {
            return htmlHelper;
        }
        public dynamic BuildExpression(string text)
        {
            return MVCControlsToolkit.Linq.FilterBuilder<M>.BuildAccessExpression(text);
        }
        public string ValidationType { get { return validationType;} }
        public string ModelPrefix { get { return modelPrefix; } }
        public IBindingsBuilder<N> ToType<N>()
        {
            if (typeof(N) == typeof(M)) return this as IBindingsBuilder<N>;
            return new BindingsBuilder<N>(writer, modelName, modelPrefix, validationType, hiddenId, htmlHelper);
        }
        public IAncestorBindingsBuilder<N, M> Parent<N>()
        {
            return new AncestorBindingsBuilder<N, M>(this, writer, modelName, modelPrefix, validationType, hiddenId, htmlHelper, "$parent");
        }
        public IAncestorBindingsBuilder<N, M> Root<N>()
        {
            return new AncestorBindingsBuilder<N, M>(this, writer, modelName, modelPrefix, validationType, hiddenId, htmlHelper, "$root");
        }
        public IAncestorBindingsBuilder<N, M> Parents<N>(int i)
        {
            return new AncestorBindingsBuilder<N, M>(this, writer, modelName, modelPrefix, validationType, hiddenId, htmlHelper, string.Format("$parents[{0}]", i));
        }
        public void AddServerErrors(string prefix)
        {
            string truePrefix = htmlHelper.ViewData["_TruePrefix_"] as string;
            if (truePrefix != null)
            {
                if (truePrefix == string.Empty)
                {
                    prefix = prefix.Replace(ClientTemplateHelper.templateSymbol + ".A.", string.Empty);
                }
                else
                {
                    prefix=prefix.Replace(ClientTemplateHelper.templateSymbol + ".A", truePrefix);
                }
            }
            ModelStateDictionary errors= new ModelStateDictionary();
            BasicHtmlHelper.CopyRelevantNonEmptyErrors(
                errors,
                htmlHelper.ViewData.ModelState,
                prefix
                );
            if (errors.Count == 0) return;
            if (errorsWriter == null)
            {
                errorsWriter = new StringBuilder();
                errorsWriter.Append(
                @"
                    <script language='javascript' type='text/javascript'>
                    $(document).ready(function()
                    {
                        MvcControlsToolkit_ServerErrors([
                    ");
            }
            else errorsWriter.Append(", ");
            bool start = true;
            foreach (KeyValuePair<string, ModelState> element in errors)
            {
                if (!start) errorsWriter.Append(", ");
                start = false;
                errorsWriter.Append(
                @"
                    {
                        id: 
                 ");
                errorsWriter.Append("'");
                errorsWriter.Append(BasicHtmlHelper.IdFromName(element.Key));
                errorsWriter.Append("'");
                errorsWriter.Append(", ");
                errorsWriter.Append(
                @"
                        name:  
                 ");
                errorsWriter.Append("'");
                errorsWriter.Append(element.Key);
                errorsWriter.Append("'");
                errorsWriter.Append(", ");
                errorsWriter.Append(
                @"
                        errors: [
                 ");
                bool innerStart = true;
                foreach (ModelError error in element.Value.Errors)
                {
                    if (!innerStart) errorsWriter.Append(", ");
                    innerStart = false;
                    errorsWriter.Append(
                    @"
                                       
                     ");
                    errorsWriter.Append("'");
                    errorsWriter.Append(error.ErrorMessage);
                    errorsWriter.Append("'");
                }
                errorsWriter.Append(
                @"
                                ]
                 ");
                errorsWriter.Append(
                @"
                    }");
            }
        }
        public MvcHtmlString HandleServerErrors()
        {
            if (errorsWriter == null) return MvcHtmlString.Create("");
            else
            {
                errorsWriter.Append("]);}); </script>");
                return
                    MvcHtmlString.Create(
                        errorsWriter.ToString()
                    );
            }
        }
        public string VerifyFormValid()
        {
            if (writer == null) return string.Empty;
            return string.Format(
                "if(!MvcControlsToolkit_FormIsValid('{0}', '{1}')) return;",
                hiddenId,
                validationType
                );
        }
        public string VerifyFieldsValid<F>(
            Expression<Func<M, F>> expression, 
            params LambdaExpression[] otherExpressions)
        {
            if (writer == null) return string.Empty;
            string firstParam =
                string.Format(
                " if(!MvcControlsToolkit_Validate('{0}', '{1}')) return; ",
                BasicHtmlHelper.IdFromName(GetFullName(expression)),
                validationType);
            if (otherExpressions != null && otherExpressions.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(firstParam);
                foreach (LambdaExpression expr in otherExpressions)
                {
                    sb.Append(
                        string.Format(
                            " if(!MvcControlsToolkit_Validate('{0}', '{1}')) return; ",
                            BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(
                                modelPrefix, 
                                ExpressionHelper.GetExpressionText(expr))),
                            validationType)
                        );
                }
                return sb.ToString();
            }
            else
                return firstParam;
        }
        public LambdaExpression L<F>(Expression<Func<M, F>> expression)
        {
            return expression;
        }
        public IBindingsBuilder<M> Add(string binding)
        {
            if (writer == null) return this;
            if (sb == null) sb = new StringBuilder();
            if (sb.Length > 0) sb.Append(", ");
            sb.Append(binding);
            return this;
        }
        public IBindingsBuilder<M> AddMethod(string name, string javaScriptCode)
        {
            
            if (writer != null) 
                writer.Write(string.Format(methodScript, modelName, name, javaScriptCode));
            return this;
        }
        public IBindingsBuilder<M> CSS<F>(
            string className,
            Expression<Func<M, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (string.IsNullOrWhiteSpace(className)) throw (new ArgumentNullException("className"));
            if (writer == null) return this;
            if (cssStringBuilder == null) cssStringBuilder = new StringBuilder();
            cssStringBuilder.Append(BindingsExtensions.standardString<M, F>(this, className, expression, format, otherExpressions));
            return this;
        }
        
        public IBindingsBuilder<M> Style<F>(
            string stylePropertyName,
            Expression<Func<M, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions )
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (string.IsNullOrWhiteSpace(stylePropertyName)) throw (new ArgumentNullException("styleePropertyName"));
            if (writer == null) return this;
            if (styleStringBuilder == null) styleStringBuilder = new StringBuilder();
            styleStringBuilder.Append(BindingsExtensions.standardString<M, F>(this, stylePropertyName, expression, format, otherExpressions));
            return this;
        }
        public IBindingsBuilder<M> Attr<F>(
            string attrName,
            Expression<Func<M, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (string.IsNullOrWhiteSpace(attrName)) throw (new ArgumentNullException("attrName"));
            if (writer == null) return this;
            if (attrStringBuilder == null) attrStringBuilder = new StringBuilder();
            attrStringBuilder.Append(BindingsExtensions.standardString<M, F>(this, attrName, expression, format, otherExpressions));
            return this;
        }
        public IBindingsBuilder<M> Event<F>(
            string eventName,
            Expression<Func<M, F>> expression,
            bool bubble=true,
            string format = null,
            params LambdaExpression[] otherExpressions)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (string.IsNullOrWhiteSpace(eventName)) throw (new ArgumentNullException("eventName"));
            if (writer == null) return this;
            if (eventStringBuilder == null) eventStringBuilder = new StringBuilder();
            eventStringBuilder.Append(BindingsExtensions.standardString<M, F>(this, eventName, expression, format, otherExpressions));
            if (!bubble) this.Add(eventName+"Bubble: false");
            return this;
        }
        public string GetFullName<F>(Expression<Func<M, F>> expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            return BasicHtmlHelper.AddField(modelPrefix, ExpressionHelper.GetExpressionText(expression));
        }
        public string GetFullBindingName(LambdaExpression expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string res = BasicHtmlHelper.AddField(bindingPrefix, ExpressionHelper.GetExpressionText(expression));
            if (string.IsNullOrWhiteSpace(res)) return ("$data");
            return res;
        }
        public MvcHtmlString Get()
        {
            if (writer == null) return MvcHtmlString.Create(string.Empty);
            if (cssStringBuilder != null && cssStringBuilder.Length > 0)
            {
                Add("css: {"); sb.Append(cssStringBuilder.ToString()); sb.Append("}");
            }
            cssStringBuilder = null;
            if (styleStringBuilder != null && styleStringBuilder.Length > 0)
            {
                Add("style: {"); sb.Append(styleStringBuilder.ToString()); sb.Append("}");
            }
            styleStringBuilder = null;
            if (attrStringBuilder != null && attrStringBuilder.Length > 0)
            {
                Add("attr: {"); sb.Append(attrStringBuilder.ToString()); sb.Append("}");
            }
            attrStringBuilder = null;
            if (eventStringBuilder != null && eventStringBuilder.Length > 0)
            {
                Add("event: {"); sb.Append(eventStringBuilder.ToString()); sb.Append("}");
            }
            eventStringBuilder = null;
            if (sb != null)
            {
                string res = sb.ToString();
                sb = null;
                return MvcHtmlString.Create(res);
            }
            else 
                return MvcHtmlString.Create(string.Empty);
        }
    }
    internal class AncestorBindingsBuilder<M, A> : BindingsBuilder<M>, IAncestorBindingsBuilder<M, A>
    {
        protected IBindingsBuilder<A> baseBuilder = null;
        public IBindingsBuilder<A> Back()
        {
            return baseBuilder; 
        }
        public AncestorBindingsBuilder(IBindingsBuilder<A> baseBuilder, System.IO.TextWriter writer, string modelName, string modelPrefix, string validationType, string hiddenId, HtmlHelper htmlHelper, string bindingPrefix)
                :base(writer, modelName, modelPrefix, validationType, hiddenId, htmlHelper, bindingPrefix)
        {
            this.baseBuilder = baseBuilder;
        }
    }
}

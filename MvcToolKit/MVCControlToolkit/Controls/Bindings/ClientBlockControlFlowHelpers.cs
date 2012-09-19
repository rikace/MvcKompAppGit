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
using System.Web.Routing;
using System.Web;

namespace MVCControlsToolkit.Controls.Bindings
{
    public static class ClientBlockControlFlowHelpers
    {
        private static void htmlPush(string stackName, string html)
        {
            Stack<string> stack;
            if (HttpContext.Current.Items.Contains(stackName))
            {
                stack = HttpContext.Current.Items[stackName] as Stack<string>;
            }
            else
            {
                stack = new Stack<string>();
                HttpContext.Current.Items[stackName] = stack;
            }
            stack.Push(html);
        }
        private static int helperLevel(HtmlHelper oldH, out bool nested)
        {
            int currValue=0;
            if (HttpContext.Current.Items.Contains("_TemplateLevel_"))
            {
                currValue = (int)HttpContext.Current.Items["_TemplateLevel_"];
            }
            currValue++;
            HttpContext.Current.Items["_TemplateLevel_"] = currValue;

            nested = oldH.ViewData["_TemplateLevel_"] != null;

            return currValue;
        }
        private static string withScript = "template: {{if: {0}, data: {0}, templateEngine: ko.nativeTemplateEngine.instance, afterRender: {5}, " +
                                    "templateOptions: {{Single:true, ModelPrefix: &quot;{1}&quot;, ModelId: &quot;{2}&quot;, ItemPrefix: &quot;&quot;, templateSymbol: &quot;{3}&quot;}}, " +
                                    "processingOptions: {{unobtrusiveClient: {4}}}}}";
        private static string forScript = "template: {{foreach: {0}, templateEngine: ko.nativeTemplateEngine.instance, " +
                                    "afterAdd: {5}, beforeRemove: {6}, afterRender: {7}, afterAllRender: {8}, " +
                                    "templateOptions: {{ModelPrefix: &quot;{1}&quot;, ModelId: &quot;{2}&quot;, ItemPrefix: &quot;&quot;, templateSymbol: &quot;{3}&quot;}}, " +
                                    "processingOptions: {{unobtrusiveClient: {4}}}}}";
        private static string ifScript = "template: {{{0}, templateEngine: ko.nativeTemplateEngine.instance, afterRender: {2}, " +
                                    "processingOptions: {{unobtrusiveClient: {1}}}}}";
        public static HtmlHelper<F> _foreach<T, F>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, IEnumerable<F>>> expression,
            ExternalContainerType itemsContainer = ExternalContainerType.koComment,
            object htmlAttributes = null,
            string afterAdd=null,
            string beforeRemove=null,
            string afterRender=null,
            string afterAllRender = null
            )
            where F: class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IDictionary<string, object> attributes = null;
            if (htmlAttributes == null)
            {
                attributes = new RouteValueDictionary();
            }
            else if (htmlAttributes is IDictionary<string, object>)
            {
                attributes = htmlAttributes as IDictionary<string, object>;
            }
            else
            {
                attributes = new RouteValueDictionary(htmlAttributes);
            }
            IBindingsBuilder<T> bindings = htmlHelper.ClientBindings();
            if (bindings == null) throw (new ArgumentNullException("bindings"));

            F model = default(F);
            Type basicType = typeof(F);
            if (basicType.IsClass)
            {
                var constructor = basicType.GetConstructor(new Type[0]);
                
                if (constructor!=null) model = (F)constructor.Invoke(new object[0]);
            }
            string bindingFieldName = bindings.GetFullBindingName(expression);
            string fieldName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            bool nested;
            int hl = helperLevel(htmlHelper, out nested);
            string templateSymbol = ClientTemplateHelper.templateSymbol + hl.ToString();
            IBindingsBuilder<F> contentBindings = new BindingsBuilder<F>(htmlHelper.ViewContext.Writer, string.Empty,
                templateSymbol + ".A",
                bindings.ValidationType, null, htmlHelper);
            ViewDataDictionary<F> dic = new ViewDataDictionary<F>(model);
            dic["ClientBindings"] = contentBindings;
            dic["_TemplateLevel_"] = hl;
            HttpContext.Current.Items["_TemplateLevel_" + hl.ToString()] = contentBindings;
            dic.TemplateInfo.HtmlFieldPrefix = templateSymbol + ".A";
            HtmlHelper<F> newHelper = new TemplateInvoker<F>().BuildHelper(htmlHelper, dic);

            string bindingsValue = string.Format(
                forScript,
                bindingFieldName,
                fieldName,
                BasicHtmlHelper.IdFromName(fieldName),
                templateSymbol,
                bindings.ValidationType == "UnobtrusiveClient" ? "true" : "false",
                afterAdd == null ? "null" : afterAdd,
                beforeRemove == null ? "null" : beforeRemove,
                afterRender == null ? "null" : afterRender,
                afterAllRender == null ? "null" : afterAllRender
                );
            if (attributes.ContainsKey("data-bind"))
            {
                attributes["data-bind"] = (attributes["data-bind"] as string) + ", " + bindingsValue;
            }
            else
                attributes["data-bind"] = bindingsValue;
            attributes["data-nobinding"] = "true";
            if (!nested && bindings.ValidationType == "UnobtrusiveClient")
            {
                bindings.AddServerErrors(fieldName);
            }
            string openTag;
            string closeTag;
            BasicHtmlHelper.GetContainerTags(itemsContainer, attributes, out openTag, out closeTag);
            htmlPush("_ClientControlsFlowStartStack_", openTag);
            htmlPush("_ClientControlsFlowStack_", closeTag);
            string innerHtml = string.Empty;
            return newHelper;
        }
        
        public static HtmlHelper<F> _with<T, F>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, F>> expression,
            ExternalContainerType itemsContainer = ExternalContainerType.koComment,
            object htmlAttributes = null,
            string afterRender=null)
            where F : class, new()
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IDictionary<string, object> attributes = null;
            if (htmlAttributes == null)
            {
                attributes = new RouteValueDictionary();
            }
            else if (htmlAttributes is IDictionary<string, object>)
            {
                attributes = htmlAttributes as IDictionary<string, object>;
            }
            else
            {
                attributes = new RouteValueDictionary(htmlAttributes);
            }
            IBindingsBuilder<T> bindings = htmlHelper.ClientBindings();
            if (bindings == null) throw (new ArgumentNullException("bindings"));
            
            
            
            ////////// template instantation ///////////
            F model = default(F);
            try
            {
               model = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            
            string bindingFieldName = bindings.GetFullBindingName(expression);
            string fieldName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
            bool nested;
            int hl = helperLevel(htmlHelper, out nested);
            string templateSymbol = ClientTemplateHelper.templateSymbol + hl.ToString();
            IBindingsBuilder<F> contentBindings = new BindingsBuilder<F>(htmlHelper.ViewContext.Writer, string.Empty,
                templateSymbol+".A",
                bindings.ValidationType, null, htmlHelper);
            ViewDataDictionary<F> dic = new ViewDataDictionary<F>(model);
            dic["ClientBindings"] = contentBindings;
            dic["_TemplateLevel_"] = hl;
            HttpContext.Current.Items["_TemplateLevel_" + hl.ToString()] = contentBindings;
            dic.TemplateInfo.HtmlFieldPrefix = templateSymbol + ".A";
            HtmlHelper<F> newHelper = new TemplateInvoker<F>().BuildHelper(htmlHelper, dic);
            

            //////////
            string bindingsValue = string.Format(
                withScript,
                bindingFieldName,
                fieldName,
                BasicHtmlHelper.IdFromName(fieldName),
                templateSymbol,
                bindings.ValidationType == "UnobtrusiveClient" ? "true" : "false",
                afterRender == null ? "null" : afterRender
                );
            if (attributes.ContainsKey("data-bind"))
            {
                attributes["data-bind"] = (attributes["data-bind"] as string) + ", " + bindingsValue;
            }
            else
                attributes["data-bind"] = bindingsValue;
            attributes["data-nobinding"] = "true";
            if (!nested && bindings.ValidationType == "UnobtrusiveClient")
            {
                bindings.AddServerErrors(fieldName);
            }
            string openTag;
            string closeTag;
            BasicHtmlHelper.GetContainerTags(itemsContainer, attributes, out openTag, out closeTag);
            htmlPush("_ClientControlsFlowStartStack_", openTag);
            htmlPush("_ClientControlsFlowStack_", closeTag);
            string innerHtml = string.Empty;
            return newHelper;
        }
        private static MvcHtmlString _cond<T, F>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, F>> expression,
             ExternalContainerType itemsContainer = ExternalContainerType.koComment,
            object htmlAttributes = null,
            string afterRender = null,
            bool negateCondition = false,
            string format=null,
            params LambdaExpression[] otherExpressions)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IDictionary<string, object> attributes = null;
            if (htmlAttributes == null)
            {
                attributes = new RouteValueDictionary();
            }
            else if (htmlAttributes is IDictionary<string, object>)
            {
                attributes = htmlAttributes as IDictionary<string, object>;
            }
            else
            {
                attributes = new RouteValueDictionary(htmlAttributes);
            }
            IBindingsBuilder<T> bindings = htmlHelper.ClientBindings();
            if (bindings == null) throw (new ArgumentNullException("bindings"));
            string bindingsValue = string.Format(
                ifScript,
                bindings.standardString(negateCondition ? "ifnot" : "if", expression, format, otherExpressions),
                bindings.ValidationType == "UnobtrusiveClient" ? "true" : "false",
                 afterRender == null ? "null" : afterRender);
            if (attributes.ContainsKey("data-bind"))
            {
                attributes["data-bind"] = (attributes["data-bind"] as string) + ", " + bindingsValue;
            }
            else
                attributes["data-bind"] = bindingsValue;
            attributes["data-nobinding"] = "true";
            string openTag;
            string closeTag;
            BasicHtmlHelper.GetContainerTags(itemsContainer, attributes, out openTag, out closeTag);
            htmlPush("_ClientControlsFlowStack_", closeTag);
            return MvcHtmlString.Create(
                    openTag
                );
        }
        public static MvcHtmlString _if<T, F>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, F>> expression,
            ExternalContainerType itemsContainer = ExternalContainerType.koComment,
            object htmlAttributes = null,
            string afterRender=null,
            string format = null,
            params LambdaExpression[] otherExpressions
            )
        {
            return _cond(htmlHelper, expression, itemsContainer, htmlAttributes, afterRender, false, format, otherExpressions);
        }
        public static MvcHtmlString _ifnot<T, F>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, F>> expression,
            ExternalContainerType itemsContainer = ExternalContainerType.koComment,
            object htmlAttributes = null,
            string afterRender=null,
            string format = null,
            params LambdaExpression[] otherExpressions
            )
        {
            return _cond(htmlHelper, expression, itemsContainer, htmlAttributes, afterRender, true, format, otherExpressions);
        }
        public static MvcHtmlString _end<T>(this HtmlHelper<T> htmlHelper)
        {
            return MvcHtmlString.Create(
                    (HttpContext.Current.Items["_ClientControlsFlowStack_"] as Stack<string>).Pop()
                );
        }
        public static MvcHtmlString _begin<T>(this HtmlHelper<T> htmlHelper)
        {
            return MvcHtmlString.Create(
                    (HttpContext.Current.Items["_ClientControlsFlowStartStack_"] as Stack<string>).Pop()
                );
        }
    }
}

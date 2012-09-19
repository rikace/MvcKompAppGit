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
using System.Globalization;

namespace MVCControlsToolkit.Controls.Bindings
{
    public class TemplatesStack
    {
        private static string key = "_TemplateStack_";
        private StringBuilder sb = new StringBuilder();
        private bool inner = false;
        
        public bool Check()
        {
            bool res = inner;
            inner = true;
            return res;
        }
        public bool Add(string x)
        {
            bool res = inner;
            inner = true;
            if (x == null) return res;
            sb.Append(Environment.NewLine);
            sb.Append(x);
            return res;
        }
        public string Get()
        {
            string res = sb.ToString();
            sb = new StringBuilder();
            inner = false;
            return res;
        }
        private static TemplatesStack getInstance()
        {
            TemplatesStack res = null;
            if (HttpContext.Current.Items.Contains(key))
            {
                res = HttpContext.Current.Items[key] as TemplatesStack;
            }
            else
            {
                res = new TemplatesStack();
                HttpContext.Current.Items[key] = res;
            }
            return res;
        }
        public static bool AddTemplate(string x)
        {
            return getInstance().Add(x);
        }
        public static bool CheckTemplates()
        {
            TemplatesStack instance = getInstance();
            bool res = instance.Check();
            return res;
        }
        public static string GetTemplates()
        {
            return getInstance().Get();
        }
        
    }
    public static class ClientBlockRepeaterHelper
    {
        public static MvcHtmlString ClientBlockRepeater<T, F>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, IEnumerable<F>>> expression,
            object template,
            ExternalContainerType itemsContainer = ExternalContainerType.div,
            object htmlAttributes = null,
            F itemPrototype = null,
            string overrideTemplateName=null,
            string afterAdd = null,
            string beforeRemove = null,
            string afterRender = null,
            string afterAllRender = null,
            object templateOptions = null,
            bool applyClientValidation = true,
            bool fastNoJavaScript = false,
            string templateEngine=null,
            string templateChoice=null
            )
            where F: class
            where T:class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (template == null) throw (new ArgumentNullException("template"));
            string templateName = null;
            if (overrideTemplateName != null)
            {
                templateName = overrideTemplateName;
            }
            else
            {
                string origName = templateName = "_Repeater_Template_";
                int i = 0;
                if (HttpContext.Current.Items.Contains(templateName))
                {
                    i = ((int)(HttpContext.Current.Items[templateName]))+1;
                    templateName = templateName + i.ToString();
                }
                HttpContext.Current.Items[origName] = i;
            }
                StringBuilder sb=new StringBuilder();
            
            bool writeTemplates = !TemplatesStack.CheckTemplates();
            try
            {
                string allTemplates = null;
                bool multiple = false;
                if (template is object[])
                {
                    multiple = true;
                    if (templateChoice == null) throw (new ArgumentNullException("templateChoice"));
                    StringBuilder tb = new StringBuilder();
                    object[] templates = template as object[];
                    for (int i = 0; i < templates.Length; i++)
                    {
                        tb.Append(htmlHelper.ClientTemplate<F>(templateName+i.ToString(CultureInfo.InvariantCulture), templates[i], true, itemPrototype).ToString());
                    }
                    allTemplates = tb.ToString();
                }
                else
                {
                    allTemplates = htmlHelper.ClientTemplate<F>(templateName, template, true, itemPrototype).ToString();
                }
                TemplatesStack.AddTemplate(allTemplates);
                sb.Append(Environment.NewLine);
                sb.Append(
                    htmlHelper.ClientBlockRepeater<T, IEnumerable<F>>(
                    multiple ? "@"+templateChoice : templateName,
                        expression,
                        itemsContainer,
                        htmlAttributes,
                        afterAdd,
                        beforeRemove,
                        afterRender,
                        afterAllRender,
                        templateOptions,
                        applyClientValidation,
                        fastNoJavaScript,
                        templateEngine).ToString());
            }
            finally
            {
                if (writeTemplates) sb.Append(TemplatesStack.GetTemplates());
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        public static MvcHtmlString ClientBlockRepeater<T, F>(
            this HtmlHelper<T> htmlHelper,
            string templateName,
            Expression<Func<T, F>> expression,
            ExternalContainerType itemsContainer = ExternalContainerType.div,
            object htmlAttributes = null,
            string afterAdd = null,
            string beforeRemove = null,
            string afterRender = null,
            string afterAllRender = null,
            object templateOptions = null,
            bool applyClientValidation = true,
            bool fastNoJavaScript = false,
            string templateEngine=null
            )
            where T: class
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (string.IsNullOrWhiteSpace(templateName)) throw (new ArgumentNullException("templateName"));
            IDictionary<string, object> attributes=null;
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
            bool writeTemplates = !TemplatesStack.CheckTemplates();
            IBindingsBuilder<T> bindings = htmlHelper.ClientBindings();
            if (bindings == null) throw (new ArgumentNullException("bindings"));
            string templates = string.Empty;
            try
            {
                string bindingsValue = bindings.Template(
                templateName,
                expression,
                afterAdd,
                beforeRemove,
                afterRender,
                templateOptions,
                null,
                applyClientValidation,
                fastNoJavaScript,
                afterAllRender, 
                templateEngine).Get().ToString();

                if (attributes.ContainsKey("data-bind"))
                {
                    attributes["data-bind"] = (attributes["data-bind"] as string) + ", " + bindingsValue;
                }
                else
                    attributes["data-bind"] = bindingsValue;
                attributes["data-nobinding"] = "true";
            }
            finally
            {
                if (writeTemplates) templates=TemplatesStack.GetTemplates();
            }
            string openTag;
            string closeTag;
            BasicHtmlHelper.GetContainerTags(itemsContainer, attributes, out openTag, out closeTag);
            return MvcHtmlString.Create(openTag+closeTag+templates);
        }
        
    }
}

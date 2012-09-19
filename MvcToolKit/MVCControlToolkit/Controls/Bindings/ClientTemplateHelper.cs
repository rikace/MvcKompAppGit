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
using System.Web;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls;
using System.Text.RegularExpressions;


namespace MVCControlsToolkit.Controls.Bindings
{
    public static class ClientTemplateHelper
    {
        
        private static string clientTemplateScript= @"
            {2}
            <script id='{0}' type='text/html'>
                  {1}
            </script>
            {3}    
            ";
        private static string prototypeFormat = @"
            <script language='javascript' type='text/javascript'>
                function {0}() {{return ko.mapping.fromJS({1});}}
            </script>
        ";
        public static string templateSymbol = "QS_23459_86745_ZU";
        //private static string quoteSymbol = "QS_23459_86745_QQ";
        internal static string scriptSymbol = "agkcvriopjvss";
        public static void AddToTemplateGlobalEval(
            this HtmlHelper htmlHelper,
            MvcHtmlString item)
        {
            if (!htmlHelper.ViewContext.HttpContext.Items.Contains("GlobalEvalsRequired"))
            {
                htmlHelper.ViewContext.HttpContext.Items["GlobalEvalsRequired"] = new StringBuilder();
            }
            StringBuilder sb = htmlHelper.ViewContext.HttpContext.Items["GlobalEvalsRequired"] as StringBuilder;
            sb.Append(item.ToString());
        }
       
        
        public static MvcHtmlString ClientTemplate<T>(this HtmlHelper htmlHelper, string uniqueName, object template, bool? applyAutomaticBindings=null, T prototype=null)
        where T: class
        {
            if (string.IsNullOrWhiteSpace(uniqueName)) throw new ArgumentNullException("uniqueName");
            if (template==null) throw new ArgumentNullException("template");
            htmlHelper.ViewContext.HttpContext.Items["ClientTemplateOn"] = new object();
            bool oldValidation = htmlHelper.ViewContext.ClientValidationEnabled;
            if (!MvcEnvironment.UnobtrusiveAjaxOn(htmlHelper)) htmlHelper.ViewContext.ClientValidationEnabled = false;
            FormContext oldFormContext = htmlHelper.ViewContext.FormContext;
            htmlHelper.ViewContext.FormContext=new FormContext(); 
            string validationType = null;
            bool automaticBindings = false;
            if (applyAutomaticBindings.HasValue) automaticBindings = applyAutomaticBindings.Value;
            else automaticBindings = htmlHelper.ViewData["ClientBindings"]!=null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }
            IBindingsBuilder<T> bindings = new BindingsBuilder<T>(htmlHelper.ViewContext.Writer, string.Empty, templateSymbol + "0.A", validationType, null, htmlHelper);
            string stringTemplate = new TemplateInvoker<T>(template, bindings).InvokeVirtual(htmlHelper, templateSymbol + "0.A");
            if (automaticBindings)
            {
                
                stringTemplate = new KoAutomaticBinder<T>(stringTemplate,
                    bindings).ToString();
            }
            else
                stringTemplate = new KoAutomaticBinder<T>(stringTemplate, null).ToString();
           /* stringTemplate = stringTemplate.Replace(templateSymbol + ".A", "${MvcControlsToolkit_TemplateName($item)}")
                .Replace(templateSymbol + "_A", "${MvcControlsToolkit_TemplateId($item)}");*/
          /*  stringTemplate = stringTemplate.Replace(templateSymbol + ".A", templateSymbol+"0.A")
                .Replace(templateSymbol + "_A", templateSymbol+"0_A");   */
            string globalEvals = string.Empty;
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("GlobalEvalsRequired"))
            {
                StringBuilder globalEvalsBuilder = htmlHelper.ViewContext.HttpContext.Items["GlobalEvalsRequired"]
                    as StringBuilder;
                globalEvals = globalEvalsBuilder.ToString();
                htmlHelper.ViewContext.HttpContext.Items.Remove("GlobalEvalsRequired");
            }
            string prototypeDeclaration = string.Empty;
            if (prototype != null)
            {
                ModelTranslator<T> model = new ModelTranslator<T>();
                model.ImportFromModel(prototype);
                prototypeDeclaration = string.Format(prototypeFormat, uniqueName, model.JSonModel);
            }
            MvcHtmlString res= MvcHtmlString.Create(string.Format(clientTemplateScript, 
                uniqueName,
                stringTemplate,
                globalEvals,
                prototypeDeclaration
                ));
            htmlHelper.ViewContext.FormContext=oldFormContext; 
            htmlHelper.ViewContext.ClientValidationEnabled=oldValidation;
            htmlHelper.ViewContext.HttpContext.Items.Remove("ClientTemplateOn");
            return res;
        }
    }
    public  class KoAutomaticBinder<T>
        where T: class
    {
        private static Regex suitableTag =
            new Regex(@"(?<closescript>\<\/\s*(?<tagName>((script)|(SCRIPT)))[^\>\<]*\>)|(?<openscript>\<\s*(?<tagName>((script)|(SCRIPT)))(?<scriptbody>[^\>\<]*)\>)|(?<tag>\<\s*(?<tagName>\p{L}+)\s+[^\>\<]*" + ClientTemplateHelper.templateSymbol + @"(?<index>[0-9]+)?\.A(?<name>[\w\._\$]+)([^\w\._\$][^\>\<]*)?\>)", RegexOptions.Compiled);
        private static Regex scriptTag =
            new Regex(@"\<(?<close>\/)?\s*((script)|(SCRIPT))(?<scriptbody>[^\>\<]*)\>", RegexOptions.Compiled);
        private static string scriptSubstitute=
            "<${close}" + ClientTemplateHelper.scriptSymbol + " ${scriptbody}>";
        private static Regex body = new Regex(@"\<\s*[fF][oO][rR][mM][^\>\<]*\>", RegexOptions.Compiled);
        private static Regex suitableAttribute = new
            Regex(@"(?<attributeName>[\w-]+)\s*=\s*((?<attributeValue>[\w.-_\$]+)|((?<quote>[\'" + "\\\"" + @"]?)(?<attributeValue>[^\'" + "\\\"" + @"]*)\k<quote>))", RegexOptions.Compiled);
        private string input;
        private int scriptCount;
        private IBindingsBuilder<T> bindings;
        private string namePrefix;
        private string addToBody;
        public KoAutomaticBinder(string input, IBindingsBuilder<T> bindings, string namePrefix=null)
        {
            this.input = input;
            this.bindings = bindings;
            scriptCount = 0;
            this.namePrefix = namePrefix;
        }
        public KoAutomaticBinder(string input, IBindingsBuilder<T> bindings, string namePrefix, string addToBody)
        {
            this.input = input;
            this.bindings = bindings;
            scriptCount = 0;
            this.namePrefix = namePrefix;
            this.addToBody = addToBody;
        }
        private string TagMatchEvaluator(Match match)
        {
            string result = match.ToString();
            var tagInfos = match.Groups["tagName"];
            string otag = tagInfos.Value;
            string tag = otag.ToLower();
            if (tag.StartsWith("!")) return result;
            if (tag == "script")
            {
                Group bodyMatch = match.Groups["scriptbody"];
                if (bodyMatch.Success)
                {
                    scriptCount++;
                    return string.Format("<{0} {1}>", namePrefix == null ? ClientTemplateHelper.scriptSymbol : "script", bodyMatch.Value);
                }
                else
                {
                    scriptCount--;
                    return string.Format("</{0}>", namePrefix == null ? ClientTemplateHelper.scriptSymbol : "script");
                }
            }
            if (scriptCount != 0) return result;
            string toAdd = null;
            
            Group nameGroup = match.Groups["name"];
            string name = null;
            if (nameGroup.Success )
            {
                name = nameGroup.Captures[0].Value;
                int index = name.IndexOf(".");
                if (index >= 0)
                {
                    name = name.Substring(index + 1);
                }
                else
                    return result;
            }
            else return result;
            string oldName=null;
            if (name.Length >= 7 && name.Substring(name.Length - 7) == "_hidden")
            {
                oldName = name;
                name = name.Substring(0, name.Length - 7);
            }
            dynamic currBindings = bindings;
             Group indexGroup = match.Groups["index"];
             if (indexGroup.Success)
             {
                 string key = "_TemplateLevel_" + indexGroup.Value;
                 if (HttpContext.Current.Items.Contains(key))
                 {
                     currBindings = HttpContext.Current.Items[key];
                 }
             }
            dynamic expression = currBindings.BuildExpression(name);
            if (expression == null)
            {
                if (oldName != null)
                {
                    expression = currBindings.BuildExpression(oldName);
                }
                if (expression == null) return result;
            }
            bool tryMultiselect=false;
            bool tryCheckBox=false;
            if (tag == "select")
            {
                tryMultiselect = true;
            }
            else if (tag == "input")
            {
                tryCheckBox = true;
            }
            MatchCollection attributes = suitableAttribute.Matches(result);
            bool look=true;
            bool elementispart = false;
            bool element_type = false;
            string dataBind = null;
            Match dataBindMatch = null;
            foreach (Match m in attributes)
            {
                Group g =  m.Groups["attributeName"];
                Group v = m.Groups["attributeValue"];
                if (g.Success && g.Value == "data-bind")
                {
                    dataBind = v.Value;
                    dataBindMatch = m;
                }
                if (g.Success && g.Value == "data-nobinding" )
                    return result;
                if (g.Success && (g.Value == "data-elementispart" ))
                    elementispart = true;
                if (g.Success && (g.Value == "data-element-type"))
                    element_type = true;
                if (look && tryMultiselect)
                {
                    if (g.Success && g.Value.ToLower() == "multiple" && v.Success && v.Value.ToLower() == "multiple")
                    {
                        toAdd = BindingsExtensions.SelectedOptions(currBindings, expression).Get().ToString();
                        look = false;
                    }
                }
                else if (look && tryCheckBox)
                {
                    if (g.Success && g.Value.ToLower() == "type" && v.Success && (v.Value.ToLower() == "checkbox" || v.Value.ToLower() == "radio"))
                    {
                        toAdd = BindingsExtensions.Checked(currBindings, expression).Get().ToString();
                        look = false;
                    }
                }
            }
            if (tag != "input" && tag != "select" && tag != "textarea" && !element_type) return result;
            if (toAdd == null && (element_type || !elementispart))
            {
                toAdd = BindingsExtensions.Value(currBindings, expression).Get().ToString();
            }

            int start = result.IndexOf(otag) + otag.Length;
            string ending = result.Substring(start);
            if (!string.IsNullOrWhiteSpace(dataBind))
            {
                string header = ending.Substring(0, dataBindMatch.Index - start);
                string footer = ending.Substring(dataBindMatch.Index + dataBindMatch.Length - start);

                result = string.Format("<{0} data-nobinding='true' data-bind=\"{1}, {2}\" {3} {4}", otag, dataBind, toAdd, header, footer);
            }
            else
                result=string.Format("<{0} data-nobinding='true' data-bind=\"{1}\" {2}", otag, toAdd, ending);
            return result;
        }
        
        public override string ToString()
        {
            if (namePrefix == null)
            {
                if (bindings != null)
                    return suitableTag.Replace(input, TagMatchEvaluator);
                else
                    return scriptTag.Replace(input, scriptSubstitute);
            }
            else
            {
                string res;
                
                if (string.IsNullOrWhiteSpace(namePrefix))
                {
                    res = suitableTag.Replace(input, TagMatchEvaluator).Replace(ClientTemplateHelper.templateSymbol + ".A.", string.Empty)
                        .Replace(ClientTemplateHelper.templateSymbol + "_A_", string.Empty);
                }
                else
                {
                    res = suitableTag.Replace(input, TagMatchEvaluator).Replace(ClientTemplateHelper.templateSymbol + ".A", namePrefix)
                    .Replace(ClientTemplateHelper.templateSymbol + "_A", BasicHtmlHelper.IdFromName(namePrefix));
                }
                if (addToBody != null)
                {
                    Match bodyTag = body.Match(res);
                    if (bodyTag.Success)
                    {
                        int split = bodyTag.Index+bodyTag.Length;
                        res = string.Format("{0} {1} {2}", res.Substring(0, split), addToBody, res.Substring(split));
                    }
                }
                return res;
            }
        }
        
    }
}

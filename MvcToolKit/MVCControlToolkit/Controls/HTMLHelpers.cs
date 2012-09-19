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
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using System.ComponentModel.DataAnnotations;
using MVCControlsToolkit.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using MVCControlsToolkit.Controls.Bindings;


namespace MVCControlsToolkit.Controls
{
    public enum ItemType { Simple, AutoDelete, HandlesDelete }
    
    public static class CoreHTMLHelpers
    {
        static CoreHTMLHelpers()
        {
            dateRewrite=new Regex("\""+@"\\/Date\(((-)?\d+)(?:[-+]\d+)?\)\\/"+"\"");
        }
        private static Regex dateRewrite;
        private static string ajaxSubmitEnablerScript = @"
            <script language='javascript' type='text/javascript'>
                 $(document).ready(function () {{
                     $('#{0}').submit(function (event) {{ return false; }})
                 }})
            </script>
        ";
        private static string antiFlicker = @"
            <script language='javascript' type='text/javascript'>
                 $(document).ready(function () {{
                     setTimeout(function(){{$('.{0}').removeClass();}}, 0);
                 }})
            </script>
        ";
        private static string toJavaScriptScript =
            @"
            <script language='javascript' type='text/javascript'>
                var {0} = {1};   
            </script>
            ";
        
        public static MvcHtmlString AntiFlicker<VM>(
            this HtmlHelper<VM> htmlHelper,
            string cssClass)
        {
            return MvcHtmlString.Create(
                string.Format(antiFlicker, cssClass)    
            );
        }
        public static MvcHtmlString TemplateFor<VM>(
            this HtmlHelper<VM> htmlHelper,
            object template,
            Type subClass = null,
            bool clientModel = false,
            string uniqueName = null,
            string externalContainerId = null)
        {
            if (template == null) throw new ArgumentNullException("template");
            return htmlHelper.TemplateFor<VM, VM>(m => m, template, subClass, clientModel, uniqueName, externalContainerId);
        }
        public static MVCControlsToolkit.Controls.Bindings.IBindingsBuilder<T> ClientBindings<T>(this HtmlHelper<T> htmlHelper)
        {
            MVCControlsToolkit.Controls.Bindings.IBindingsBuilderBase act = htmlHelper.ViewData["ClientBindings"] as MVCControlsToolkit.Controls.Bindings.IBindingsBuilderBase;
            if (act == null) return null;
            MVCControlsToolkit.Controls.Bindings.IBindingsBuilder<T> res = act.ToType<T>();
            res.SetHelper(htmlHelper);
            return res;
        }

        public static HtmlHelper<T> CrossHelper<T>(
            this HtmlHelper htmlHelper,
            T crossViewModel, string prefix = "")
        {
            ViewDataDictionary<T> dataDictionary = new ViewDataDictionary<T>(crossViewModel);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = prefix;
            return new TemplateInvoker<T>().BuildHelper(htmlHelper, dataDictionary);
        }
        public static MvcHtmlString Template<VM, T>(
           this HtmlHelper<VM> htmlHelper,
           T model,
           object template,
           bool clientModel = false,
           string uniqueName = null,
           string externalContainerId = null,
           string prefix="",
           bool initialSave = false)
            where T: class, new()
        {
            if (template == null) throw new ArgumentNullException("template");
            if (model== null) throw new ArgumentNullException("model");
            string partialPrefix = prefix;
            if (clientModel)
            {
                if (uniqueName == null) throw (new ArgumentNullException("uniqueName"));
                if (externalContainerId == null) throw (new ArgumentNullException("externalContainerId"));

                IBindingsBuilder<T> bindings = BindingsHelpers.InnerClientViewModel<T>(
                                htmlHelper,
                                uniqueName,
                                model,
                                partialPrefix,
                                prefix,
                                externalContainerId,
                                initialSave,
                                true,
                                true
                    );
                return MvcHtmlString.Create(
                        new KoAutomaticBinder<T>(
                            new TemplateInvoker<T>(template, bindings).Invoke(htmlHelper, model, ClientTemplateHelper.templateSymbol + ".A"),
                            bindings,
                            prefix).ToString()
                    );
            }
            else
            {
                return MvcHtmlString.Create(
                new TemplateInvoker<T>(template).Invoke(
                htmlHelper,
                model,
                prefix));
            }
        }
        public static MvcHtmlString TemplateFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, T>> expression,
            object template, 
            Type subClass=null,
            bool clientModel=false,
            string uniqueName=null,
            string externalContainerId=null,
            bool initialSave=false)
        {
            if (template == null) throw new ArgumentNullException("template");
            if (expression == null) throw new ArgumentNullException("expression");
            IBindingsBuilder<VM> contextBindings = htmlHelper.ClientBindings();
            if (contextBindings != null) clientModel = false;
            if (clientModel )
            {
                if (uniqueName == null) throw (new ArgumentNullException("uniqueName"));
                if (externalContainerId == null) throw (new ArgumentNullException("externalContainerId")); 
            }
            string partialPrefix = ExpressionHelper.GetExpressionText(expression);
            string prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(
                partialPrefix);
            T model = default(T);
            try
            {
                model = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }

            if (subClass != null || clientModel || contextBindings != null)
            {
                if (subClass == null) subClass = typeof(T);
                if (typeof(T).IsAssignableFrom(subClass))
                {
                    if (clientModel)
                    {
                        object bindings = typeof(BindingsHelpers).GetMethod("InnerClientViewModel", BindingFlags.Static | BindingFlags.NonPublic)
                            .MakeGenericMethod(new Type[] { subClass })
                            .Invoke(null, new object[] {
                                htmlHelper,
                                uniqueName,
                                model,
                                partialPrefix,
                                prefix,
                                externalContainerId,
                                initialSave,
                                true, 
                                true
                        });
                        return MvcHtmlString.Create(
                        typeof(KoAutomaticBinder<object>)
                            .GetGenericTypeDefinition()
                            .MakeGenericType(new Type[] { subClass })
                            .GetConstructor(new Type[] { typeof(string), bindings.GetType(), typeof(string) })
                            .Invoke(new object[]{
                            (typeof(TemplateInvoker<string>)
                                .GetGenericTypeDefinition()
                                .MakeGenericType(new Type[] { subClass })
                                .GetConstructor(new Type[]{typeof(object), bindings.GetType()})
                                .Invoke(new object[]{template, bindings}) as ITemplateInvoker)
                                .Invoke(htmlHelper, model, ClientTemplateHelper.templateSymbol+".A", prefix),
                            bindings,
                            prefix
                        }).ToString()
                        );
                    }
                    else
                    {
                        if (contextBindings == null)
                        {
                            return MvcHtmlString.Create(
                                (typeof(TemplateInvoker<string>)
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(new Type[] { subClass })
                                    .GetConstructor(new Type[] { typeof(object) })
                                    .Invoke(new object[] { template }) as ITemplateInvoker)
                                    .Invoke(htmlHelper, model, prefix)
                                );
                        }
                        else
                        {
                            object bindings =
                                typeof(BindingsBuilder<object>)
                            .GetGenericTypeDefinition()
                            .MakeGenericType(new Type[] { subClass })
                            .GetConstructor(new Type[] { typeof(System.IO.TextWriter), typeof(string), typeof(string), typeof(string), typeof(string), typeof(HtmlHelper), typeof(string) })
                            .Invoke(new object[] { htmlHelper.ViewContext.Writer, contextBindings.ModelName, BasicHtmlHelper.AddField(partialPrefix, contextBindings.ModelPrefix), contextBindings.ValidationType, null, htmlHelper, BasicHtmlHelper.AddField(partialPrefix,contextBindings.BindingPrefix) });
                            return MvcHtmlString.Create(
                                (typeof(TemplateInvoker<string>)
                                    .GetGenericTypeDefinition()
                                    .MakeGenericType(new Type[] { subClass })
                                    .GetConstructor(new Type[] { typeof(object), bindings.GetType() })
                                    .Invoke(new object[] { template, bindings }) as ITemplateInvoker)
                                    .Invoke(htmlHelper, model, prefix)
                                );
                        }
                    }
                }
                else
                {
                    throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    subClass.FullName,
                    typeof(T).FullName));
                }
            }
            
            return MvcHtmlString.Create(
                new TemplateInvoker<T>(template).Invoke(
                htmlHelper,
                model,
                prefix));
        }
        /*
        public static MVCControlsToolkit.Controls.Bindings.BindingsContext<VM, T> ClientBindingsBlock<VM, T>(
            this HtmlHelper<VM> htmlHelper, 
            Expression<Func<VM, T>> expression,
            string uniqueName,
            string externalContainerId = null
            )
            where T: class, new()
        {
            if (uniqueName == null) throw (new ArgumentNullException("uniqueName"));
            if (expression == null) throw (new ArgumentNullException("expression"));
            string partialPrefix = ExpressionHelper.GetExpressionText(string.Empty);
            string prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
            T model = default(T);
            try
            {
                model = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            IBindingsBuilder<T> bindings= BindingsHelpers.InnerClientViewModel<T>(
                htmlHelper,
                uniqueName,
                model,
                partialPrefix,
                prefix,
                externalContainerId,
                true, true, true);
            System.IO.TextWriter writer = htmlHelper.ViewContext.Writer;
            htmlHelper.ViewContext.Writer = new System.IO.StringWriter();
            htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix=ClientTemplateHelper.templateSymbol+".A";
            htmlHelper.ViewData["ClientBindings"] = bindings;
            return new MVCControlsToolkit.Controls.Bindings.BindingsContext<VM, T>(bindings,
                prefix,
                htmlHelper,
                writer);

            
        }*/
        public static MvcHtmlString ToJavaScript(this HtmlHelper htmlHelper, object data, string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName)) throw (new ArgumentNullException("variableName"));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string JSonModel = serializer.Serialize(data) ;
            JSonModel = dateRewrite.Replace(JSonModel, "new Date($1)");
            return 
                MvcHtmlString.Create(
                    string.Format(toJavaScriptScript, variableName, JSonModel));
        }
        public static void DeclareStringArray(this HtmlHelper htmlHelper, string[] strings, string name, bool clientRenderNow=false)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            if (strings == null || strings.Length == 0) throw (new ArgumentNullException("strings"));
            htmlHelper.ViewContext.HttpContext.Items["StringArray_" + name] = strings;
            if (clientRenderNow &&
                !htmlHelper.ViewContext.HttpContext.Items.Contains("StringArrayRendered_" + name))
            {
                htmlHelper.ViewContext.HttpContext.Items["StringArrayRendered_" + name] = new object();
                htmlHelper.ViewContext.Writer.WriteLine(htmlHelper.ToJavaScript(strings, name));
            }
        }
        public static string[] GetStringArray(this HtmlHelper htmlHelper, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            string[] res = null;
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("StringArray_" + name))
            {
                res = htmlHelper.ViewContext.HttpContext.Items["StringArray_" + name] as string[];
                if ((htmlHelper.ViewContext.HttpContext.Items.Contains("ClientTemplateOn") || htmlHelper.ViewData["ClientBindings"] != null) &&
                    ! htmlHelper.ViewContext.HttpContext.Items.Contains("StringArrayRendered_" + name))
                {
                    htmlHelper.ViewContext.HttpContext.Items["StringArrayRendered_" + name] = new object();
                    htmlHelper.AddToTemplateGlobalEval(
                         htmlHelper.ToJavaScript(res, name));
                }
            }
            return res;
        }
        public static MvcHtmlString IsValid<VM>(this HtmlHelper<VM> htmlHelper)
        {
            return htmlHelper.GenericInput(InputType.Hidden, "IsValid",
                htmlHelper.ViewData.ModelState.IsValid ? "True" : "False", null);
        }
        public static MvcHtmlString AjaxSubmitEnabler<VM>(this HtmlHelper<VM> htmlHelper, string formName, bool addPrefix=true)
        {
            if (addPrefix) formName=BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(formName));
            return MvcHtmlString.Create(
                string.Format(ajaxSubmitEnablerScript, formName));
        }
        public static string PrefixedName<VM>(this HtmlHelper<VM> htmlHelper, string localName)
        {
            return BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, localName);
        }
        public static string PrefixedName<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression)
        {
            return htmlHelper.PrefixedName(ExpressionHelper.GetExpressionText(expression));
        }
        public static string PrefixedId<VM>(this HtmlHelper<VM> htmlHelper, string localName)
        {
            return BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, localName));
        }
        public static string PrefixedId<VM,T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression)
        {
            return htmlHelper.PrefixedId(ExpressionHelper.GetExpressionText(expression));
        }
        public static string ItemContainerId<VM>(this HtmlHelper<VM> htmlHelper)
        {
            return htmlHelper.PrefixedId("Container");
        }
        public static string StandardIdFor<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression)
        {
            return htmlHelper.PrefixedId(ExpressionHelper.GetExpressionText(expression));
        }
        public static string ValueFor<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string res=string.Empty;
            try
            {
                res=Convert.ToString(expression.Compile().Invoke(htmlHelper.ViewData.Model));
            }
            catch
            {
            }
            return res;
        }
        public static MvcHtmlString _P<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression)
        {
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("ClientTemplateOn"))
            {
                return MvcHtmlString.Create("${" + ExpressionHelper.GetExpressionText(expression) + "}");
            }
            else
            {
                T res = default(T);
                try{
                        res= expression.Compile().Invoke(htmlHelper.ViewData.Model) ;
                        return MvcHtmlString.Create(res.ToString());
                }
                catch{
                    return MvcHtmlString.Create(string.Empty);
                }
                
            }
        }
        
        private static string clientFormattingScript =
        "${{MvcControlsToolkit_FormatDisplay({0}(), '{1}', {2}, '{3}', '{4}', '{5}')}}";
        
        public static MvcHtmlString _F<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression)
        {
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("ClientTemplateOn"))
            {
                string clientFormat;
                string prefix;
                string postfix;
                string nullText;
                int clientType = CoreHTMLHelpers.
                    GetClientInfo(expression, out clientFormat, out prefix, out postfix, out nullText);
                MvcHtmlString res = MvcHtmlString.Create(
                    string.Format(clientFormattingScript,
                        ExpressionHelper.GetExpressionText(expression),
                        clientFormat,
                        clientType,
                        prefix,
                        postfix,
                        nullText)
                    );
                return res;
            }
            else
                return htmlHelper.FormattedDisplay(expression);
            
        }
        private static string clientDisplayFieldScript =
        "<span data-bind=\"text: (function(){{ MvcControlsToolkit_FormatDisplay({0}(), '{1}', {2}, '', '', ''); return MvcControlsToolkit_FormatDisplay({0}(), '{1}', {2}, '{3}', '{4}', '{5}');}})()\" id = '{6}_D' data-element-type = 'DisplayField'  {7}></span>" +
        @"<script language='javascript' type='text/javascript'>
              var {6}_True = null;
        </script> ";
        private static string clientDisplayFieldEnumScript =
        "<span data-bind=\"text: (function(){{MvcControlsToolkit_FormatDisplay({0}(), '', 0, '', '', ''); return MvcControlsToolkit_GetArrayString({0}(), '{1}', {2});}})()\" id = '{3}_D' data-element-type = 'DisplayField'  {4}></span>" +
        @"<script language='javascript' type='text/javascript'>
              var {3}_True = null;
        </script> ";
        private static string clientDisplayFieldImgScript =
        "<img data-bind=\"attr: {{src: (function(){{MvcControlsToolkit_FormatDisplay({0}(), '', 0, '', '', ''); return MvcControlsToolkit_GetArrayString({0}(), '{1}', {2});}})()}}\" id = '{3}_D' data-element-type = 'DisplayField' {4}/>" +
        @"<script language='javascript' type='text/javascript'>
              var {3}_True =null;
        </script> ";
        private static string clientDisplayFieldImgAltScript =
        "<img  data-bind=\"attr: {{src: (function(){{MvcControlsToolkit_FormatDisplay({0}(), '', 0, '', '', ''); return MvcControlsToolkit_GetArrayString({0}(), '{1}', {2});}})(), alt: (function(){{return MvcControlsToolkit_GetArrayString({0}(), '{5}', {2});}})()}}\" id = '{3}_D' data-element-type = 'DisplayField' {4}/>" +
        @"<script language='javascript' type='text/javascript'>
              var {3}_True =null;
        </script> ";
        
        public static MvcHtmlString _D<VM, T>(
            this HtmlHelper<VM> htmlHelper, 
            Expression<Func<VM, T>> expression,
            IDictionary<string, object> htmlAttributes = null, 
            string valuesArrayName=null,
            string urlsArrayName=null)
        {
            string[] displayValues = null;
            string[] imageUrls = null;
            if (valuesArrayName != null) displayValues = htmlHelper.GetStringArray(valuesArrayName);
            if (urlsArrayName != null) imageUrls = htmlHelper.GetStringArray(urlsArrayName);
            if (htmlHelper.ViewContext.HttpContext.Items.Contains("ClientTemplateOn") || htmlHelper.ClientBindings()!=null)
            {
                string fieldName = ExpressionHelper.GetExpressionText(expression);
                string id = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName));
                bool isNullable=false;
                bool useArrays = false;
               
                if (typeof(T) == typeof(bool) || typeof(T).IsEnum) 
                {
                    useArrays=true; 
                }
                else if (typeof(T) == typeof(bool?))
                {
                    useArrays = true;
                    isNullable = true;
                }
                
                if (useArrays && imageUrls != null && imageUrls.Length != 0)
                {
                    if (displayValues != null && displayValues.Length != 0)
                    {
                        return MvcHtmlString.Create(
                                 string.Format(
                                 clientDisplayFieldImgAltScript,
                                 fieldName,
                                 urlsArrayName,
                                 isNullable ? "true" : "false",
                                 id,
                                 BasicHtmlHelper.GetAttributesString(htmlAttributes),
                                 valuesArrayName));

                    }
                    else
                    {
                        return MvcHtmlString.Create(
                                 string.Format(
                                 clientDisplayFieldImgScript,
                                 fieldName,
                                 urlsArrayName,
                                 isNullable ? "true" : "false",
                                 id,
                                 BasicHtmlHelper.GetAttributesString(htmlAttributes)));
                    }
                }
                else if (useArrays && displayValues != null && displayValues.Length != 0)
                {
                    return MvcHtmlString.Create(
                                 string.Format(
                                 clientDisplayFieldEnumScript,
                                 fieldName,
                                 valuesArrayName,
                                 isNullable ? "true" : "false",
                                 id,
                                 BasicHtmlHelper.GetAttributesString(htmlAttributes)));
                }
                string clientFormat;
                string prefix;
                string postfix;
                string nullText;
                int clientType = CoreHTMLHelpers.
                    GetClientInfo(expression, out clientFormat, out prefix, out postfix, out nullText);
                return MvcHtmlString.Create(
                    string.Format(clientDisplayFieldScript,
                        fieldName,
                        clientFormat,
                        clientType,
                        prefix,
                        postfix,
                        nullText,
                        id,
                        BasicHtmlHelper.GetAttributesString(htmlAttributes))
                    );
            }
            else
                return htmlHelper.DisplayField(expression, displayValues, imageUrls, htmlAttributes);

        }
        public static MvcHtmlString FormattedDisplay<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression, string format=null, string nullString="")
        {
            if(expression == null) throw(new ArgumentNullException("expression"));
            string fieldName = ExpressionHelper.GetExpressionText(expression);
            T res = default(T);
            try
            {
                res = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            if (format != null)
            {
                if (res != null) return MvcHtmlString.Create(htmlHelper.Encode(string.Format(format, res)));
                else return MvcHtmlString.Create(htmlHelper.Encode(nullString));
            }
            PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(VM));
            FormatAttribute[] fa = pa[typeof(FormatAttribute)] as FormatAttribute[];
            if (fa != null && fa.Length > 0) return MvcHtmlString.Create(htmlHelper.Encode(fa[0].GetDisplay(res)));
            DisplayFormatAttribute[] dfa = pa[typeof(DisplayFormatAttribute)] as DisplayFormatAttribute[];
            if (dfa != null && dfa.Length > 0) return MvcHtmlString.Create(htmlHelper.Encode(new FormatAttribute(dfa[0]).GetDisplay(res)));
            return MvcHtmlString.Create(htmlHelper.Encode(Convert.ToString(res)));
        }
        private static string displayFieldScript =
           @"
            <span id = '{1}' data-element-type = 'DisplayField' {3}>{0}</span>
            <script language='javascript' type='text/javascript'>
              var {1}_True = '{2}';
            </script>   
            ";
        private static string displayImageEnumlFieldScript =
           @"
            <img id = '{1}' data-element-type = 'DisplayField' src='{0}'/>
            <script language='javascript' type='text/javascript'>
              var {1}_True = '{2}';
            </script>   
            ";
        private static string displayImageAltEnumlFieldScript =
             @"
            <img id = '{1}' data-element-type = 'DisplayField' src='{0}' alt = '{3}' />
            <script language='javascript' type='text/javascript'>
              var {1}_True = '{2}';
            </script>   
            ";
        private static string displayBoolFieldScript =
           @"
            <input id='{1}' type='checkbox' {0} />
            <script language='javascript' type='text/javascript'>
              var {1}_True = '{2}';
            </script>   
            ";
        public static int GetClientInfo<T, F>(
            Expression<Func<T, F>> expression,
            out string clientFormat,
            out string prefix,
            out string postfix, 
            out string nullString)
        {
            prefix = string.Empty;
            postfix = string.Empty;
            clientFormat = string.Empty;
            nullString = string.Empty;
            if (typeof(T) != typeof(F) && (typeof(F) == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(typeof(F))))
            {
                PropertyAccessor pa = new PropertyAccessor(
                    ExpressionHelper.GetExpressionText(expression),
                    typeof(T));
                FormatAttribute[] fa = pa[typeof(FormatAttribute)] as FormatAttribute[];

                if (fa != null && fa.Length > 0)
                {
                    fa[0].GetClientFormat(out prefix, out postfix, out clientFormat);
                    nullString = fa[0].NullDisplayText;
                }
            }
            return GetClientType(typeof(F));
        }
        public static string ClientFormatting<M, T>(this HtmlHelper<M> htmlHelper, Expression<Func<M, T>> expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string prefix;
            string postfix;
            string format;
            string nullString;
            int clientType;
            clientType = GetClientInfo(expression, out format, out prefix, out postfix, out nullString);
            return
                    string.Format("{{\"prefix\": \"{0}\", \"postfix\": \"{1}\", \"format\": \"{2}\", \"nullstring\": \"{3}\", \"type\": {4}}}",
                    prefix, postfix, format, nullString, clientType);
                ;

        }
        public static int GetClientType(Type TargetType)
        {
            if (TargetType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(TargetType))
            {
                Type gType = TargetType.GetInterface("IEnumerable`1");
                if (gType != null) TargetType = gType.GetGenericArguments()[0];
                else TargetType = TargetType.GetGenericArguments()[0];
            }
            int value = 0;
            if (TargetType == typeof(string))
            {
                value = 0;
            }
            else if (TargetType == typeof(DateTime))
            {
                value = 4;
            }
            else if (TargetType == typeof(Nullable<DateTime>))
            {
                value = 4;
            }
            else if (TargetType == typeof(Int32))
            {
                value = 2;
            }
            else if (TargetType == typeof(Nullable<Int32>))
            {
                value = 2;
            }
            else if (TargetType == typeof(Int16))
            {
                value = 2;
            }
            else if (TargetType == typeof(Nullable<Int16>))
            {
                value = 2;
            }
            else if (TargetType == typeof(Int64))
            {
                value = 2;
            }
            else if (TargetType == typeof(Nullable<Int64>))
            {
                value = 2;
            }
            else if (TargetType == typeof(UInt32))
            {
                value = 1;
            }
            else if (TargetType == typeof(Nullable<UInt32>))
            {
                value = 1;
            }
            else if (TargetType == typeof(UInt16))
            {
                value = 1;
            }
            else if (TargetType == typeof(Nullable<UInt16>))
            {
                value = 1;
            }
            else if (TargetType == typeof(UInt64))
            {
                value = 1;
            }
            else if (TargetType == typeof(Nullable<UInt64>))
            {
                value = 1;
            }
            else if (TargetType == typeof(byte))
            {
                value = 1;
            }
            else if (TargetType == typeof(Nullable<byte>))
            {
                value = 1;
            }
            else if (TargetType == typeof(sbyte))
            {
                value = 2;
            }
            else if (TargetType == typeof(Nullable<sbyte>))
            {
                value = 2;
            }
            else if (TargetType == typeof(decimal))
            {
                value = 3;
            }
            else if (TargetType == typeof(Nullable<decimal>))
            {
                value = 3;
            }
            else if (TargetType == typeof(float))
            {
                value = 3;
            }
            else if (TargetType == typeof(Nullable<float>))
            {
                value = 3;
            }
            else if (TargetType == typeof(double))
            {
                value = 3;
            }
            else if (TargetType == typeof(Nullable<double>))
            {
                value = 3;
            }
            else
            {
                value = -1;
            }
            return value;
        }
        private static MvcHtmlString DisplayEnum<VM>(HtmlHelper<VM> htmlHelper, object value, int index, string id, string[] displayValues = null, string[] imageUrls = null, IDictionary<string, object> htmlAttributes=null)
        {
            if (imageUrls != null && index < imageUrls.Length && imageUrls[index] != null)
            {
                if (displayValues != null && index < displayValues.Length && displayValues[index] != null)
                {
                    return MvcHtmlString.Create(
                            string.Format(displayImageAltEnumlFieldScript,
                            imageUrls[index],
                            id,
                            Convert.ToString(value),
                            htmlHelper.Encode(displayValues[index])));
                }
                else
                {
                    return MvcHtmlString.Create(
                            string.Format(displayImageEnumlFieldScript,
                            imageUrls[index],
                            id,
                            Convert.ToString(value)));
                }
            }
            else if (displayValues != null && index < displayValues.Length && displayValues[index] != null)
            {
                return MvcHtmlString.Create(
                        string.Format(displayFieldScript,
                        htmlHelper.Encode(displayValues[index]),
                        id,
                        Convert.ToString(value), BasicHtmlHelper.GetAttributesString(htmlAttributes)));
            }
            else
            {
                
                return MvcHtmlString.Create(
                        string.Format(displayFieldScript,
                        htmlHelper.Encode(Convert.ToString(value)),
                        id,
                        Convert.ToString(value), BasicHtmlHelper.GetAttributesString(htmlAttributes)));
            }
        }
        public static MvcHtmlString DisplayField<VM, T>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, T>> expression, 
            string[] displayValues=null, string[] imageUrls=null, IDictionary<string, object> htmlAttributes=null, string formattedValue=null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string fieldName = ExpressionHelper.GetExpressionText(expression);
            string fRes = null;
            bool mustEncode = true;
            T res = default(T);
            try
            {
                res = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            string id = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName));
            if (typeof(T) == typeof(bool))
            {
                bool val = Convert.ToBoolean(res);
                int index = val ? 1 : 0;
                if (displayValues == null && imageUrls == null)
                {
                    return MvcHtmlString.Create(
                        string.Format(displayBoolFieldScript,
                        val ? "checked = ' checked'" : "",
                        id,
                        Convert.ToString(val)));
                }
                else
                {
                    return DisplayEnum(htmlHelper, val, index, id, displayValues, imageUrls, htmlAttributes);
                }
            }
            else if (typeof(T) == typeof(bool?))
            {
                bool? val = res as bool?;
                int index = 0;
                if (val != null & val.HasValue)
                    index = val.Value ? 2 : 1;
                return DisplayEnum(htmlHelper, val, index, id, displayValues, imageUrls, htmlAttributes);
            }
            else if (typeof(T).IsEnum)
            {
                object val = res;
                int index = Convert.ToInt32(val);
                return DisplayEnum(htmlHelper, val, index, id, displayValues, imageUrls, htmlAttributes);
            }
            
            
            PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(VM));
            FormatAttribute[] fa = pa[typeof(FormatAttribute)] as FormatAttribute[];
            string prefix = string.Empty;
            string postfix = string.Empty;
            string clientFormat = string.Empty;
            if (fa != null && fa.Length > 0)
            {
                mustEncode = fa[0].HtmlEncode;
                fRes = fa[0].GetDisplay(res);
                fa[0].GetClientFormat(out prefix, out postfix, out clientFormat);
            }
            else

            {
                DisplayFormatAttribute[] dfa = pa[typeof(DisplayFormatAttribute)] as DisplayFormatAttribute[];
                if (dfa != null && dfa.Length > 0)
                {
                    mustEncode = dfa[0].HtmlEncode;
                    fRes = new FormatAttribute(dfa[0]).GetDisplay(res);
                }
                else
                {
                    if (res == null)
                    {
                        fRes = string.Empty;
                        
                    }
                    else
                    {
                        fRes = BasicHtmlHelper.GetStandardValue(res);
                        
                    }
                }
            }
            if (formattedValue != null) fRes = formattedValue;

            
                
                return MvcHtmlString.Create(
                       string.Format(displayFieldScript,
                       mustEncode ? htmlHelper.Encode(fRes) : fRes, id, BasicHtmlHelper.GetStandardValue(res), BasicHtmlHelper.GetAttributesString(htmlAttributes)));
            
        }
        public static SubClassCastInvoker<VM, M> DescendatntCast<VM, M>(this HtmlHelper<VM> htmlHelper, RenderInfo<M> renderInfo)
        {
            if (renderInfo == null) throw(new ArgumentNullException("renderInfo"));
            return new SubClassCastInvoker<VM, M>(htmlHelper, renderInfo);
        }
        public static SubClassCastInvoker<VM, VM> DescendatntCast<VM>(this HtmlHelper<VM> htmlHelper)
        {
            return new SubClassCastInvoker<VM, VM>(htmlHelper,
                    new RenderInfo<VM>()
                    {
                        Model = htmlHelper.ViewData.Model,
                        Prefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                        PartialPrefix=string.Empty,
                        PartialRendering=string.Empty
                    }
                );
        }
        public static SubClassCastInvoker<VM, M> DescendatntCast<VM, M>(this HtmlHelper<VM> htmlHelper, Expression<Func<VM, M>> expression)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            return new SubClassCastInvoker<VM, M>(htmlHelper, htmlHelper.ExtractFromModel(expression));
        }
        public static HtmlHelper<NM> TransformedHelper<VM, M, NM>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, M>> expression,
           NM newModel,
           params object[] args)
            where NM : IDisplayModel
        {
            return
                RenderWith(htmlHelper,
                    InvokeTransform(htmlHelper,
                        expression,
                        newModel,
                        args));
        }
        public static MvcHtmlString TransformHelper<VM, M, NM>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, M>> expression,
           NM newModel,
           out HtmlHelper<NM> newHtmlHelper,
           params object[] args)
            where NM : IDisplayModel
        {
            return
                RenderWith(htmlHelper,
                    InvokeTransform(htmlHelper,
                        expression,
                        newModel,
                        args), out newHtmlHelper);
        }
        public static HtmlHelper<NM> TransformedUpdateHelper<VM, M, NM>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, M>> expression,
           NM newModel,
           params object[] args)
            where NM : IUpdateModel
        {
            return
                RenderWith(htmlHelper,
                    InvokeUpdateTransform(htmlHelper,
                        expression,
                        newModel,null, args));
        }
        public static MvcHtmlString TransformUpdateHelper<VM, M, NM>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, M>> expression,
           NM newModel,
           out HtmlHelper<NM> newHtmlHelper,
           params object[] args)
            where NM : IUpdateModel
        {
            return
                RenderWith(htmlHelper,
                    InvokeUpdateTransform(htmlHelper,
                        expression,
                        newModel, null, args), out newHtmlHelper);
        }
        public static RenderInfo<NM> InvokeTransform<VM, M, NM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, M>> expression,
            NM newModel,
            object[] args = null,
            bool duplicate=false)
            where NM : IDisplayModel
            
        {
            
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (newModel == null) throw (new ArgumentNullException("newModel"));
            
            var fullPropertyPath =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));
             M originalValue=default(M);
             try
            {
                originalValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch{}

            newModel.ImportFromModel(originalValue, args);
            string shortPrefix=ExpressionHelper.GetExpressionText(expression);
            return new RenderInfo<NM>
            {
                PartialRendering = BasicHtmlHelper.RenderDisplayInfo(htmlHelper, typeof(NM), shortPrefix, duplicate),
                
                Prefix = string.IsNullOrWhiteSpace(fullPropertyPath) ? "$" : fullPropertyPath+".$",

                PartialPrefix = string.IsNullOrWhiteSpace(shortPrefix) ? "$" : shortPrefix + ".$",

                Model=newModel

            };
        }

        public static RenderInfo<NM> InvokeTransform<VM, M, NM>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<M> renderInfo,
            NM newModel,
            object[] args = null,
            bool duplicate=false)
            where NM : IDisplayModel
            
        {
            
            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));
            if (newModel == null) throw (new ArgumentNullException("newModel"));
            
            var fullPropertyPath = renderInfo.Prefix;
            newModel.ImportFromModel(renderInfo.Model, args);

            return new RenderInfo<NM>
            {
                PartialRendering = BasicHtmlHelper.RenderDisplayInfo(htmlHelper, typeof(NM), renderInfo.PartialPrefix, duplicate),

                Prefix = string.IsNullOrWhiteSpace(fullPropertyPath) ? "$" : fullPropertyPath + ".$",

                PartialPrefix = string.IsNullOrWhiteSpace(renderInfo.PartialPrefix) ? "$" : renderInfo.PartialPrefix + ".$",

                Model=newModel

            };
        }
        public static RenderInfo<NM> InvokeTransformExt<VM, M, NM>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<M> renderInfo,
            NM newModel,
            object[] args = null,
            bool duplicate = false)
            where NM : IDisplayModel
        {

            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));
            if (newModel == null) throw (new ArgumentNullException("newModel"));

            var fullPropertyPath = renderInfo.Prefix;
            newModel.ImportFromModel(renderInfo.Model, args);

            return new RenderInfo<NM>
            {
                PartialRendering = BasicHtmlHelper.RenderDisplayInfo(htmlHelper, newModel==null ? typeof(NM) : newModel.GetType(), renderInfo.PartialPrefix, duplicate),

                Prefix = string.IsNullOrWhiteSpace(fullPropertyPath) ? "$" : fullPropertyPath + ".$",

                PartialPrefix = string.IsNullOrWhiteSpace(renderInfo.PartialPrefix) ? "$" : renderInfo.PartialPrefix + ".$",

                Model = newModel

            };
        }
        public static RenderInfo<NM> InvokeUpdateTransform<VM, M, NM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, M>> expression,
            NM newModel,
            string[] expressions=null,
            object[] pars=null,
            bool evalExpressions=true)
            where NM : IUpdateModel
            
        {
            if (newModel == null) throw (new ArgumentNullException("newModel"));
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (expressions == null) expressions = new string[0];

            
            string partialPrefix = ExpressionHelper.GetExpressionText(expression);
            string toRender = BasicHtmlHelper.RenderUpdateInfo<M>(htmlHelper, newModel, ref partialPrefix, expressions);
            var prefix =
                 htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                     partialPrefix);
            object[] args = new object[expressions.Length];
           
            M model=default(M);
            try
            {
                model = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            if (model != null && evalExpressions)
            {
                int index = 0;
                foreach (string exp in expressions)
                {
                    try
                    {
                        args[index] = new PropertyAccessor(model, exp).Value;
                    }
                    catch
                    {
                    }
                    
                    index++;
                }
            }

            newModel.ImportFromModel(model, args, expressions, pars);

            return new RenderInfo<NM>
            {
                Prefix = string.IsNullOrWhiteSpace(prefix) ? "$" : prefix+".$",
                PartialPrefix = string.IsNullOrWhiteSpace(partialPrefix) ? "$" : partialPrefix + ".$",
                Model = newModel,
                PartialRendering = toRender
            };
        }

        public static RenderInfo<NM> InvokeUpdateTransform<VM, M, NM>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<M> renderInfo,
            NM newModel,
            string[] expressions = null,
            object[] pars=null,
            bool evaluateExpressions=true)
            where NM : IUpdateModel
        {
            if (newModel == null) throw (new ArgumentNullException("newModel"));
            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));
            
                
            if (expressions == null) expressions = new string[0];

            var prefix = renderInfo.Prefix.Trim();
            string partialPrefix = renderInfo.PartialPrefix.Trim();
            

            string toRender = renderInfo.PartialRendering + BasicHtmlHelper.RenderUpdateInfo<M>(htmlHelper, newModel, ref partialPrefix, expressions);
            prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
            object[] args = new object[expressions.Length];
            
            M model = renderInfo.Model;

            if (model != null && evaluateExpressions)
            {
                int index = 0;
                foreach (string exp in expressions)
                {
                    try
                    {
                        args[index] = new PropertyAccessor(model, exp).Value;
                    }
                    catch
                    {
                    }

                    index++;
                }
            }

            newModel.ImportFromModel(model, args, expressions, pars);

            return new RenderInfo<NM>
            {
                Prefix = string.IsNullOrWhiteSpace(prefix) ? "$" : prefix + ".$",
                PartialPrefix = string.IsNullOrWhiteSpace(partialPrefix) ? "$" : prefix + ".$",
                Model = newModel,
                PartialRendering = toRender
            };
        }

        public static MvcHtmlString RenderIn<VM, M>(
            this HtmlHelper<VM> htmlHelper,
            object template,
            RenderInfo<M> renderiNFO)
        {
            if (template == null) throw (new ArgumentNullException("template"));
            if (renderiNFO == null) throw (new ArgumentNullException("renderiNFO"));

            if (template == null) template = typeof(M).Name;

            ViewDataDictionary<M> dataDictionary =
                new ViewDataDictionary<M>(renderiNFO.Model);
                dataDictionary.TemplateInfo.HtmlFieldPrefix = renderiNFO.Prefix;
                BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);    
                return MvcHtmlString.Create(
                    renderiNFO.PartialRendering+
                    new TemplateInvoker<M>(template).Invoke<VM>(htmlHelper, dataDictionary));
        }

        public static HtmlHelper<M> RenderWith<VM, M>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<M> renderiNFO)
        {
            
            if (renderiNFO == null) throw (new ArgumentNullException("renderiNFO"));
            htmlHelper.ViewContext.Writer.Write(renderiNFO.PartialRendering);

            ViewDataDictionary<M> dataDictionary =
                new ViewDataDictionary<M>(renderiNFO.Model);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = renderiNFO.Prefix;
             BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
             return new TemplateInvoker<M>().BuildHelper(htmlHelper, dataDictionary);
        }
        public static  MvcHtmlString RenderWith<VM, M>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<M> renderiNFO, 
            out HtmlHelper<M> newHtmlHelper)
        {

            if (renderiNFO == null) throw (new ArgumentNullException("renderiNFO"));

            ViewDataDictionary<M> dataDictionary =
                new ViewDataDictionary<M>(renderiNFO.Model);
            dataDictionary.TemplateInfo.HtmlFieldPrefix = renderiNFO.Prefix;
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            newHtmlHelper = new TemplateInvoker<M>().BuildHelper(htmlHelper, dataDictionary);
            return MvcHtmlString.Create(renderiNFO.PartialRendering);
        }
        public static MvcHtmlString RenderIn<VM, M, NM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<M, NM>> expression,
            object template,
            RenderInfo<M> renderiNFO)
        {
            if (template == null) throw (new ArgumentNullException("template"));
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (renderiNFO == null) throw (new ArgumentNullException("renderiNFO"));

            if (template == null) template = typeof(M).Name;
            
            NM newModel = default(NM);
            if (renderiNFO.Model != null)
            {
                try
                {
                    newModel = expression.Compile().Invoke(renderiNFO.Model);
                }
                catch { }
            }

            ViewDataDictionary<NM> dataDictionary =
                new ViewDataDictionary<NM>(newModel);

            string prefix = ExpressionHelper.GetExpressionText(expression);
            if (string.IsNullOrWhiteSpace(prefix))
                dataDictionary.TemplateInfo.HtmlFieldPrefix = renderiNFO.Prefix;
            else
                dataDictionary.TemplateInfo.HtmlFieldPrefix = renderiNFO.Prefix+"."+prefix;
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            return MvcHtmlString.Create(
                renderiNFO.PartialRendering +
                new TemplateInvoker<NM>(template).Invoke<VM>(htmlHelper, dataDictionary));
        }

        public static MvcHtmlString RenderModelEnumerableFor<VM, M>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, List<M>>> expression,
            string template,
            bool deletable = false,
            ItemType itemType = ItemType.Simple
            )
        {
            switch (itemType)
            {
                case ItemType.Simple:
                    return RenderEnumerableIn<SimpleEnumerableUpdater<M>, VM, M>
                        (htmlHelper,
                        template,
                         ExtractFromModel(htmlHelper, expression),
                         deletable);
                case ItemType.HandlesDelete:
                    return RenderEnumerableIn<EnumerableUpdater<M>, VM, M>
                        (htmlHelper,
                        template,
                         ExtractFromModel(htmlHelper, expression),
                         deletable);
                default:
                    return RenderEnumerableIn<AutoEnumerableUpdater<M>, VM, M>
                        (htmlHelper,
                        template,
                         ExtractFromModel(htmlHelper, expression),
                         deletable);
            }

        }

        public static  MvcHtmlString RenderAddItemFor<VM, NM, IT>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, NM>> expression,
            string template,
            ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Simple:
                    return htmlHelper.RenderIn(
                        template,
                        htmlHelper.Extract(
                            t => t.Item,
                            htmlHelper.InvokeUpdateTransform(expression,
                            new SimpleEnumerableUpdater<IT>())));
                case ItemType.AutoDelete:
                    return htmlHelper.RenderIn(
                        template,
                        htmlHelper.Extract(
                            t => t.Item,
                            htmlHelper.InvokeUpdateTransform(expression,
                            new AutoEnumerableUpdater<IT>())));
                default:
                    return htmlHelper.RenderIn(
                       template,
                       htmlHelper.InvokeUpdateTransform(
                           expression,
                           new EnumerableUpdater<IT>()));

            }
        }

        public static MvcHtmlString RenderAddItemFor<VM, NM, IT>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<NM> renderInfo,
            string template,
            ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Simple:
                    return htmlHelper.RenderIn(
                        template,
                        htmlHelper.Extract(
                            t => t.Item,
                            htmlHelper.InvokeUpdateTransform(renderInfo,
                            new SimpleEnumerableUpdater<IT>())));
                case ItemType.AutoDelete:
                    return htmlHelper.RenderIn(
                        template,
                        htmlHelper.Extract(
                            t => t.Item,
                            htmlHelper.InvokeUpdateTransform(renderInfo,
                            new AutoEnumerableUpdater<IT>())));
                default:
                    return htmlHelper.RenderIn(
                       template,
                       htmlHelper.InvokeUpdateTransform(
                           renderInfo,
                           new EnumerableUpdater<IT>(true)));

            }
        }

        public static MvcHtmlString RenderEnumerableFor<VM, M>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<List<M>> renderiNFO,
            string template,
            bool deletable = false,
            ItemType itemType = ItemType.Simple
            )
        {
            switch (itemType)
            {
                case ItemType.Simple:
                    return RenderEnumerableIn<SimpleEnumerableUpdater<M>, VM, M>
                        (htmlHelper,
                        template,
                         renderiNFO,
                         deletable);
                case ItemType.HandlesDelete:
                    return RenderEnumerableIn<EnumerableUpdater<M>, VM, M>
                        (htmlHelper,
                        template,
                         renderiNFO,
                         deletable);
                default:
                    return RenderEnumerableIn<AutoEnumerableUpdater<M>, VM, M>
                        (htmlHelper,
                        template,
                         renderiNFO,
                         deletable);
            }

        }
        
        public static MvcHtmlString RenderEnumerableIn<IM, VM, M>(
            HtmlHelper<VM> htmlHelper,
            string template,
            RenderInfo<List<M>> renderiNFO,
            bool deletable=false)
            
        {
            if (template == null) throw (new ArgumentNullException("template"));
            if (renderiNFO == null) throw (new ArgumentNullException("renderiNFO"));

            bool empty = false;
            IEnumerator enumerator=null;
            if (renderiNFO.Model == null) empty = true;
            else
            {
                enumerator=renderiNFO.Model.GetEnumerator();
                enumerator.Reset();
                empty=!enumerator.MoveNext();
            }
            if(empty)
            {
              return MvcHtmlString.Create(
                renderiNFO.PartialRendering);
            }
            if (template == null) template = typeof(IM).Name;
            if (deletable && typeof(IM).GetInterface("IUpdateModel") == null) 
                throw new NotSupportedException(string.Format(ControlsResources.IUpdateDisplayNotImplemented, "IM"));
            
            

            StringBuilder sb = new StringBuilder();
            sb.Append(renderiNFO.PartialRendering);

            if (deletable)
            {
                ConstructorInfo ci = typeof(IM).GetConstructor(new Type[0]);
                if (ci == null) throw new NotSupportedException(string.Format(Resources.NoConstructor0, typeof(IM).Name));
                int index = 0;
                foreach (object o in renderiNFO.Model)
                {
                    if (o == null) continue;
                    IUpdateModel um = ci.Invoke(new object[0]) as IUpdateModel;
                    um.ImportFromModel(o, null, null, new object[0]);
                    string prefix=renderiNFO.Prefix;
                    string partialPrefix = renderiNFO.PartialPrefix;
                   
                   
                    sb.Append(
                        BasicHtmlHelper.RenderUpdateInfo<M>(htmlHelper, um, ref partialPrefix, new string[0]));

                    prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);

                    if (typeof(IM) == typeof(SimpleEnumerableUpdater<M>))
                    {

                        SimpleEnumerableUpdater<M> itemModel = null;
                        if (um != null) itemModel = ((SimpleEnumerableUpdater<M>)um);
                        ViewDataDictionary<M> dataDictionary = new ViewDataDictionary<M>(itemModel.Item);
                        dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item");
                        BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                        sb.Append(htmlHelper.Partial(template, dataDictionary));
                    }
                    else if (typeof(IM) == typeof(AutoEnumerableUpdater<M>))
                    {
                        AutoEnumerableUpdater<M> itemModel = null;
                        if (um != null) itemModel = ((AutoEnumerableUpdater<M>)um);
                        ViewDataDictionary<M> dataDictionary = new ViewDataDictionary<M>(itemModel.Item);
                        dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item");
                        BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                        sb.Append(htmlHelper.Partial(template, dataDictionary));
                    }
                    else
                    {
                        ViewDataDictionary<IM> dataDictionary = new ViewDataDictionary<IM>((IM)um);
                        dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item");
                        BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                        sb.Append(htmlHelper.Partial(template, dataDictionary));
                    }
                    index++;     
                }
            }
            else
            {
                int index=0;
                foreach (object o in renderiNFO.Model)
                {
                    ViewDataDictionary<M> dataDictionary = null;
                    if(o==null)
                        dataDictionary=new ViewDataDictionary<M>(null);
                    else
                        dataDictionary=new ViewDataDictionary<M>((M)o);

                    dataDictionary.TemplateInfo.HtmlFieldPrefix = EnumerableHelper.CreateSubIndexName(renderiNFO.Prefix, index);
                    BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                    sb.Append(htmlHelper.Partial(template, dataDictionary));
                    index++;
                }
            }
            

            return MvcHtmlString.Create(
                sb.ToString());
        }

        public static RenderInfo<List<IM>> ConvertToList<VM, NM, IM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, NM>> expression)
        {
            return htmlHelper.Extract(t => t.Items,
                htmlHelper.InvokeTransform(expression, new EnumerableConverter<IM>()));

        }

        public static RenderInfo<List<IM>> ConvertToList<VM, NM, IM>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<NM> renderInfo)
        {
            return htmlHelper.Extract(t => t.Items,
                htmlHelper.InvokeTransform(renderInfo, new EnumerableConverter<IM>()));

        }

        public static RenderInfo<IEnumerable<IM>> ConvertToIEnumerable<VM, NM, IM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, NM>> expression)
        {
            RenderInfo<List<IM>> res= htmlHelper.Extract(t => t.Items,
                htmlHelper.InvokeTransform(expression, new EnumerableConverter<IM>()));
            return new RenderInfo<IEnumerable<IM>>(res.Prefix, res.PartialPrefix, res.PartialRendering, res.Model);

        }

        public static RenderInfo<IEnumerable<IM>> ConvertToIEnumerable<VM, NM, IM>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<NM> renderInfo)
        {
            RenderInfo<List<IM>> res = htmlHelper.Extract(t => t.Items,
                htmlHelper.InvokeTransform(renderInfo, new EnumerableConverter<IM>()));
            return new RenderInfo<IEnumerable<IM>>(res.Prefix, res.PartialPrefix, res.PartialRendering, res.Model);
        }
        public static  RenderInfo<NM> Extract<VM, OM, NM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<OM, NM>> expression,
            RenderInfo<OM> originalModelInfo)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (originalModelInfo == null) throw (new ArgumentNullException("originalModelInfo"));

            var fullPropertyPath = originalModelInfo.Prefix;
            string partialPath = originalModelInfo.PartialPrefix;
            var expressionName = ExpressionHelper.GetExpressionText(expression);
            if (!string.IsNullOrWhiteSpace(expressionName))
            {
                fullPropertyPath = fullPropertyPath + "." + expressionName;
                partialPath = partialPath + "." + expressionName;
            }
           
            NM finalValue = default(NM);
            try
            {
                finalValue = expression.Compile().Invoke(originalModelInfo.Model);
            }
            catch { }
            return new RenderInfo<NM>
            {
                PartialRendering = originalModelInfo.PartialRendering,

                Prefix = fullPropertyPath,

                PartialPrefix=partialPath,

                Model = finalValue
            };
        }

        public static RenderInfo<NM> ExtractFromModel<VM, NM>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, NM>> expression
            )
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            
            var fullPropertyPath =
                  htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(
                      ExpressionHelper.GetExpressionText(expression));

            var partialPath = ExpressionHelper.GetExpressionText(expression);
            NM finalValue = default(NM);
            try
            {
                finalValue = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch { }
            return new RenderInfo<NM>
            {
                PartialRendering = string.Empty,

                Prefix = fullPropertyPath,

                PartialPrefix=partialPath,

                Model = finalValue
            };
        }

        

    }
}

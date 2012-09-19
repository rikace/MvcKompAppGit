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
using System.Globalization;
using System.Web.Routing;


namespace MVCControlsToolkit.Controls
{
    public enum ContentAlign{Left, Right, Center, None};
    
    public static class TypedTextBoxHelper
    {

        private static void handleBindings(ref  IDictionary<string, object> attributeExtensions, bool edit)
        {
            
            if (attributeExtensions == null) attributeExtensions=new Dictionary<string, object>();
            if (edit) attributeExtensions["data-element-type"] = "TypedTextBox";
            else attributeExtensions["data-element-type"] = "TypedEditDisplay";
            attributeExtensions["data-elementispart"] = "true";
        }
        private static void applyAlignement(Type vtype, ContentAlign c, ref IDictionary<string, object> attributes, bool edit = true)
        {

            handleBindings(ref attributes, edit);
            string align=null;
            switch(c)
            {
                case ContentAlign.Center: align="center"; break;
                case ContentAlign.Left: align="left"; break;
                case ContentAlign.Right: align="right"; break;
                default: if (attributes == null) attributes = new Dictionary<string, object>(); attributes["data-client-type"] = getType(vtype); return;

            }
            if (attributes == null) attributes = new Dictionary<string, object>();
            BasicHtmlHelper.SetDefaultStyle(attributes, "text-align", align);
            attributes["data-client-type"] = getType(vtype);
        }
        private static void applyAlignement(Type vtype, ContentAlign c, object attributes, out IDictionary<string, object> newAttributes, bool edit = true)
        {
            newAttributes = null;
            if (attributes != null) newAttributes = BasicHtmlHelper.AnonymousObjectToDictionary(attributes);
            handleBindings(ref newAttributes, edit);
            string align = null;
            switch (c)
            {
                case ContentAlign.Center: align = "center"; break;
                case ContentAlign.Left: align = "left"; break;
                case ContentAlign.Right: align = "right"; break;
                default: if (newAttributes == null) newAttributes = new Dictionary<string, object>(); newAttributes["data-client-type"] = getType(vtype); return;

            }
            if (newAttributes == null) newAttributes = new Dictionary<string, object>();
            
            BasicHtmlHelper.SetDefaultStyle(newAttributes, "text-align", align);
            newAttributes["data-client-type"] = getType(vtype);
        }
        private static void applyFunctions<M>(HtmlHelper<M> htmlHelper,string name, IDictionary<string, object> newAttributes)
        {
            string id= BasicHtmlHelper.IdFromName(
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name));
            newAttributes["mvc-controls-toolkit-set"] = 
                string.Format("function(x) {{MvcControlsToolkit_TypedTextBox_Set('{0}_hidden', x);}}",
                id);
            newAttributes["mvc-controls-toolkit-get-true"] =
                string.Format("MvcControlsToolkit_TypedTextBox_GetTrue('{0}')",
                id);
            newAttributes["mvc-controls-toolkit-get-display"] =
                string.Format("MvcControlsToolkit_TypedTextBox_GetDisplay('{0}_hidden')",
                id);
        }
        private static int getType(Type TargetType)
        {
            int value = 0;
            if (TargetType == typeof(DateTime) || TargetType == typeof(Nullable<DateTime>)){
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
            return value;
        }

        private static string scriptCalendar= @"
        <script language='javascript' type='text/javascript'>
                MvcControlsToolkit_TypedTextBox_Init('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', {13});
        </script>
        ";
        private static string script = @"
        <script language='javascript' type='text/javascript'>
                MvcControlsToolkit_TypedTextBox_Init('{0}', '{1}', '{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', null);
        </script>
        "; 

        private static string scriptEditDisplay = @"
        <script language='javascript' type='text/javascript'>
                MvcControlsToolkit_DisplayEdit_Init('{0}', '{1}', 0, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', null, null);
        </script>
        ";

        private static string scriptEditDisplayCalendar = @"
        <script language='javascript' type='text/javascript'>
                MvcControlsToolkit_DisplayEdit_Init('{0}', '{1}', 0, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', {13}, null);
        </script>
        ";

        private static string scriptEditDisplaySelect = @"
        <script language='javascript' type='text/javascript'>
                MvcControlsToolkit_DisplayEdit_Init('{0}', '{1}', 0, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', null, 'select');
        </script>
        ";

        
        private static string getScripts<M, T>(
            HtmlHelper<M> htmlHelper,
            string name,
            string watermarkCss,
            string overrideClientFormat,
            string overrideClientPrefix,
            string overrideClientPostfix,
            string overrideWatermark,
            CalendarOptions calendarOptions = null,
            ModelMetadata model=null)
        {
            if (model==null) model = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
            if (overrideClientFormat == null)
            {
                object res;
                model.AdditionalValues.TryGetValue("MVCControlsToolkit.ClientFormatString", out res);
                overrideClientFormat = res as string;
            }
            if (string.IsNullOrWhiteSpace(overrideClientFormat)) overrideClientFormat = string.Empty;
            if (overrideClientPrefix == null)
            {
                object res;
                model.AdditionalValues.TryGetValue("MVCControlsToolkit.ClientFormatPrefix", out res);
                overrideClientPrefix = res as string;
            }
            if (string.IsNullOrWhiteSpace(overrideClientPrefix)) overrideClientPrefix = string.Empty;
            if (overrideClientPostfix == null)
            {
                object res;
                model.AdditionalValues.TryGetValue("MVCControlsToolkit.ClientFormatPostfix", out res);
                overrideClientPostfix = res as string;
            }
            if (string.IsNullOrWhiteSpace(overrideClientPostfix)) overrideClientPostfix = string.Empty;
            if (string.IsNullOrWhiteSpace(overrideWatermark))
            {
                overrideWatermark = model.Watermark;
            }
            if (string.IsNullOrWhiteSpace(overrideWatermark)) overrideWatermark = string.Empty;
            int clientType = getType(typeof(T));

            string validationType = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }

            string fieldId = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name + "_hidden"));
            string companionId = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name ));
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string decimalSeparator = ci.NumberFormat.NumberDecimalSeparator;
            string digitsSeparator = ci.NumberFormat.NumberGroupSeparator;
            string plus = ci.NumberFormat.PositiveSign;
            string minus = ci.NumberFormat.NegativeSign;
            if (calendarOptions != null && (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?)))
            {
                return string.Format(scriptCalendar, fieldId, companionId, watermarkCss, clientType, decimalSeparator, digitsSeparator,
                    plus, minus, overrideClientPrefix, overrideClientPostfix, overrideClientFormat, overrideWatermark, validationType, calendarOptions.Render('#' + fieldId, false, true));
            }
            else
            {
                return string.Format(script, fieldId, companionId, watermarkCss, clientType, decimalSeparator, digitsSeparator,
                    plus, minus, overrideClientPrefix, overrideClientPostfix, overrideClientFormat, overrideWatermark, validationType);
            }
        }
        private static string renderDisplay<M>(
            HtmlHelper<M> htmlHelper,
            string name,
            object displayAttributes)
        {
            string id =
                BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name + "_display"));
            if (displayAttributes == null) return string.Format(
                "<span id = '{0}' style='display:none'></span>",
                id);
            IDictionary<string, object> displayAttributesdic = displayAttributes as IDictionary<string, object>;
            if (displayAttributesdic == null) displayAttributesdic = BasicHtmlHelper.AnonymousObjectToDictionary(displayAttributes);
            BasicHtmlHelper.SetStyle(displayAttributesdic, "display", "none");
            return string.Format(
                "<span id = '{0}' {1}></span>",
                id,
                BasicHtmlHelper.GetAttributesString(displayAttributesdic));
        }
        private static string getEditDisplayScripts<M, T>(
            HtmlHelper<M> htmlHelper,
            string name,
            string overrideClientFormat,
            string overrideClientPrefix,
            string overrideClientPostfix,
            bool simpleClick,
            bool editEnabled,
            bool useSelect = false,
            CalendarOptions calendarOptions=null,
            ModelMetadata model=null)
        {
            if (model==null) model = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
            if (overrideClientFormat == null)
            {
                object res;
                model.AdditionalValues.TryGetValue("MVCControlsToolkit.ClientFormatString", out res);
                overrideClientFormat = res as string;
            }
            if (string.IsNullOrWhiteSpace(overrideClientFormat)) overrideClientFormat = string.Empty;
            if (overrideClientPrefix == null)
            {
                object res;
                model.AdditionalValues.TryGetValue("MVCControlsToolkit.ClientFormatPrefix", out res);
                overrideClientPrefix = res as string;
            }
            if (string.IsNullOrWhiteSpace(overrideClientPrefix)) overrideClientPrefix = string.Empty;
            if (overrideClientPostfix == null)
            {
                object res;
                model.AdditionalValues.TryGetValue("MVCControlsToolkit.ClientFormatPostfix", out res);
                overrideClientPostfix = res as string;
            }
           
            int clientType = getType(typeof(T));

            string validationType = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }

            string fieldId = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name ));
            string companionId = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name+"_display"));
            CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string decimalSeparator = ci.NumberFormat.NumberDecimalSeparator;
            string digitsSeparator = ci.NumberFormat.NumberGroupSeparator;
            string plus = ci.NumberFormat.PositiveSign;
            string minus = ci.NumberFormat.NegativeSign;
            if (calendarOptions != null && (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?)))
            {
                return string.Format(scriptEditDisplayCalendar, fieldId, companionId, clientType, decimalSeparator, digitsSeparator,
                    plus, minus, overrideClientPrefix, overrideClientPostfix, overrideClientFormat, validationType,
                    simpleClick ? "click" : "dblclick", model.NullDisplayText == null ? string.Empty : model.NullDisplayText, calendarOptions.Render('#' + fieldId, false, true));
            }
            else
            {

                return string.Format(useSelect ? scriptEditDisplaySelect : scriptEditDisplay, fieldId, companionId, clientType, decimalSeparator, digitsSeparator,
                    plus, minus, overrideClientPrefix, overrideClientPostfix, overrideClientFormat, validationType,
                    editEnabled ? (simpleClick ? "click" : "dblclick"): null, model.NullDisplayText == null ? string.Empty : model.NullDisplayText);
            }
        }
       //TypedEditDisplay Helpers
        public static MvcHtmlString TypedEditDisplay<M, T>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            IDictionary<string, object> displayAttributes,
            IDictionary<string, object> editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled=true,
            CalendarOptions calendarOptions=null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            if (displayAttributes == null) throw (new ArgumentNullException("displayAttributes"));
            applyAlignement(typeof(T), contentAlign, ref editAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes)+
                htmlHelper.TextBox(name, value, editAttributes).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix, 
                simpleClick,
                editEnabled, false, calendarOptions));
        }
        public static MvcHtmlString TypedEditDisplayFor<M, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            IDictionary<string, object> displayAttributes,
            IDictionary<string, object> editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true,
            CalendarOptions calendarOptions = null)
        {
            if (expression==null) throw (new ArgumentNullException("expression"));
            if (displayAttributes == null && editAttributes == null) throw (new ArgumentNullException("displayAttributes & editAttributes"));
            string name = ExpressionHelper.GetExpressionText(expression);


            applyAlignement(typeof(T), contentAlign, ref editAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.TextBoxFor(expression, editAttributes).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, false, calendarOptions, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        public static MvcHtmlString TypedEditDisplay<M, T>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            object displayAttributes,
            object editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true,
            CalendarOptions calendarOptions = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            if (displayAttributes == null && editAttributes == null) throw (new ArgumentNullException("displayAttributes & editAttributes"));
            IDictionary<string, object> newEditAttributes=null;
            applyAlignement(typeof(T), contentAlign, ref newEditAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.TextBox(name, value, newEditAttributes).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, false, calendarOptions));
        }
        public static MvcHtmlString TypedEditDisplayFor<M, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            object displayAttributes,
            object editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true,
            CalendarOptions calendarOptions = null)
        {
            if (expression==null) throw (new ArgumentNullException("expression"));
            if (displayAttributes == null && editAttributes == null) throw (new ArgumentNullException("displayAttributes & editAttributes"));
            string name = ExpressionHelper.GetExpressionText(expression);
            
            
            IDictionary<string, object> newEditAttributes = null;
            applyAlignement(typeof(T), contentAlign, ref newEditAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.TextBoxFor(expression, newEditAttributes).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, false, calendarOptions, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        public static MvcHtmlString TypedEditDisplay<M, T>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true,
            CalendarOptions calendarOptions = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            IDictionary<string, object> newAttributes;
            applyAlignement(typeof(T), contentAlign, null, out newAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, null) +
                htmlHelper.TextBox(name, value, newAttributes).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, false, calendarOptions));
        }
        public static MvcHtmlString TypedEditDisplayFor<M, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true,
            CalendarOptions calendarOptions = null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string name = ExpressionHelper.GetExpressionText(expression);
            
            IDictionary<string, object> newAttributes;
            applyAlignement(typeof(T), contentAlign, null, out newAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, null) +
                htmlHelper.TextBoxFor(expression, newAttributes).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, false, calendarOptions, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        //TypedEditDisplay with DropDown Helpers
        public static MvcHtmlString TypedEditDisplay<M, T, TItem, TDisplay>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            ChoiceList<TItem, T, TDisplay> items,
            IDictionary<string, object> displayAttributes,
            IDictionary<string, object> editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled=true)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            if (displayAttributes == null) throw (new ArgumentNullException("displayAttributes"));
            applyAlignement(typeof(T), contentAlign, ref editAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.DropDownList(name, value, editAttributes, items).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, true));
        }
        public static MvcHtmlString TypedEditDisplayFor<M, T, TItem, TDisplay>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            ChoiceList<TItem, T, TDisplay> items,
            IDictionary<string, object> displayAttributes,
            IDictionary<string, object> editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (displayAttributes == null && editAttributes == null) throw (new ArgumentNullException("displayAttributes & editAttributes"));
            string name = ExpressionHelper.GetExpressionText(expression);

            applyAlignement(typeof(T), contentAlign, ref editAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.DropDownListFor(expression, editAttributes, items).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, true, null, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        public static MvcHtmlString TypedEditDisplay<M, T, TItem, TDisplay>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            ChoiceList<TItem, T, TDisplay> items,
            object displayAttributes,
            object editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            if (displayAttributes == null && editAttributes == null) throw (new ArgumentNullException("displayAttributes & editAttributes"));
            IDictionary<string, object> newEditAttributes = null;
            if (editAttributes != null) newEditAttributes = new RouteValueDictionary(editAttributes);
            applyAlignement(typeof(T), contentAlign, ref newEditAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.DropDownList(name, value, newEditAttributes, items).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, true));
        }
        public static MvcHtmlString TypedEditDisplayFor<M, T, TItem, TDisplay>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            ChoiceList<TItem, T, TDisplay> items,
            object displayAttributes,
            object editAttributes,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (displayAttributes == null && editAttributes == null) throw (new ArgumentNullException("displayAttributes & editAttributes"));
            string name = ExpressionHelper.GetExpressionText(expression);
            
            IDictionary<string, object> newEditAttributes = null;
            if (editAttributes != null) newEditAttributes = new RouteValueDictionary(editAttributes);
            applyAlignement(typeof(T), contentAlign, ref newEditAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, displayAttributes) +
                htmlHelper.DropDownListFor(expression, newEditAttributes, items).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, true, null, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        public static MvcHtmlString TypedEditDisplay<M, T, TItem, TDisplay>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            ChoiceList<TItem, T, TDisplay> items,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            IDictionary<string, object> newAttributes;
            applyAlignement(typeof(T), contentAlign, null, out newAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, null) +
                htmlHelper.DropDownList(name, value, newAttributes, items).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, true));
        }
        public static MvcHtmlString TypedEditDisplayFor<M, T, TItem, TDisplay>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            ChoiceList<TItem, T, TDisplay> items,
            ContentAlign contentAlign = ContentAlign.None,
            bool simpleClick = false,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            bool editEnabled = true)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            string name = ExpressionHelper.GetExpressionText(expression);
           
            IDictionary<string, object> newAttributes;
            applyAlignement(typeof(T), contentAlign, null, out newAttributes, false);
            return MvcHtmlString.Create(
                renderDisplay(htmlHelper, name, null) +
                htmlHelper.DropDownListFor(expression, newAttributes, items).ToString() +
                getEditDisplayScripts<M, T>(
                htmlHelper,
                name,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                simpleClick,
                editEnabled, true, null, ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        //TypedTextBox helpers
        public static MvcHtmlString TypedTextBox<M, T>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            IDictionary<string, object> attributes,
            string watermarkCss = null,
            ContentAlign contentAlign = ContentAlign.None,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            string overrideWatermark = null,
            CalendarOptions calendarOptions = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            if (attributes == null) throw (new ArgumentNullException("attributes"));
            if (watermarkCss == null) watermarkCss = "Theme-TypedTextBox-Watermark";
            applyAlignement(typeof(T), contentAlign, ref attributes);
            //applyFunctions(htmlHelper, name, attributes);
            return MvcHtmlString.Create(  
                (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient ?
                htmlHelper.TextBox(name, value, new RouteValueDictionary{{ "style", "display:none"}, {"data-companionpostfix", "_hidden"}, {"data-elementispart", "true"}}).ToString()
                : htmlHelper.Hidden(name, value, new RouteValueDictionary {{ "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                ) + htmlHelper.TextBox(name + "_hidden", null, attributes).ToString() +
                getScripts<M, T>(
                htmlHelper,
                name,
                watermarkCss,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                overrideWatermark, calendarOptions));
        }
        public static MvcHtmlString TypedTextBox<M, T>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            object attributes,
            string watermarkCss = null,
            ContentAlign contentAlign = ContentAlign.None,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            string overrideWatermark = null,
            CalendarOptions calendarOptions = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw(new ArgumentNullException("name"));
            IDictionary<string, object> newAttributes;
            if (watermarkCss == null) watermarkCss = "Theme-TypedTextBox-Watermark";
            applyAlignement(typeof(T), contentAlign, attributes, out newAttributes);
            //applyFunctions(htmlHelper, name, newAttributes);
            return MvcHtmlString.Create( 
                (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient ?
                htmlHelper.TextBox(name, value, new RouteValueDictionary { { "style", "display:none" }, { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                : htmlHelper.Hidden(name, value, new RouteValueDictionary { { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                ) + htmlHelper.TextBox(name + "_hidden", value, newAttributes).ToString() +
                getScripts<M, T>(
                htmlHelper,
                name,
                watermarkCss,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                overrideWatermark, calendarOptions));
        }
        public static MvcHtmlString TypedTextBox<M, T>(this HtmlHelper<M> htmlHelper,
            string name,
            T value,
            string watermarkCss = null,
            ContentAlign contentAlign = ContentAlign.None,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            string overrideWatermark = null,
            CalendarOptions calendarOptions = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw (new ArgumentNullException("name"));
            IDictionary<string, object> newAttributes;
            if (watermarkCss == null) watermarkCss = "Theme-TypedTextBox-Watermark";
            applyAlignement(typeof(T), contentAlign, null, out newAttributes);
            //applyFunctions(htmlHelper, name, newAttributes);
            return MvcHtmlString.Create(
                (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient ?
                htmlHelper.TextBox(name, value, new RouteValueDictionary { { "style", "display:none" }, { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                : htmlHelper.Hidden(name, value, new RouteValueDictionary { { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                ) + htmlHelper.TextBox(name + "_hidden", value, newAttributes).ToString() +
                getScripts<M, T>(
                htmlHelper,
                name,
                watermarkCss,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                overrideWatermark, calendarOptions));
        }
        public static MvcHtmlString TypedTextBoxFor<M, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            IDictionary<string, object> attributes,
            string watermarkCss = null,
            ContentAlign contentAlign = ContentAlign.None,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            string overrideWatermark = null,
            CalendarOptions calendarOptions = null)
        {
            if (expression==null) throw (new ArgumentNullException("expression"));
            if (attributes == null) throw (new ArgumentNullException("attributes"));
            if (watermarkCss == null) watermarkCss = "Theme-TypedTextBox-Watermark";
            applyAlignement(typeof(T), contentAlign, ref attributes);
            
            string name=ExpressionHelper.GetExpressionText(expression);
            object value=null;
            try
            {
                value=expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            //applyFunctions(htmlHelper, name, attributes);
            return MvcHtmlString.Create(
                (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient ?
                htmlHelper.TextBoxFor(expression, new RouteValueDictionary { { "style", "display:none" }, { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                : htmlHelper.HiddenFor(expression, new RouteValueDictionary { { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                ) + htmlHelper.TextBox(name + "_hidden", value, attributes).ToString() +
                getScripts<M, T>(
                htmlHelper,
                name,
                watermarkCss,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                overrideWatermark, calendarOptions,
                ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }

        public static MvcHtmlString TypedTextBoxFor<M, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            object attributes,
            string watermarkCss = null,
            ContentAlign contentAlign = ContentAlign.None,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            string overrideWatermark = null,
            CalendarOptions calendarOptions = null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IDictionary<string, object> newAttributes;
            applyAlignement(typeof(T), contentAlign, attributes, out newAttributes);
            if (watermarkCss == null) watermarkCss = "Theme-TypedTextBox-Watermark";
            string name = ExpressionHelper.GetExpressionText(expression);
            object value = null;
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            //applyFunctions(htmlHelper, name, newAttributes);
            return MvcHtmlString.Create(
                (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient ?
                htmlHelper.TextBoxFor(expression, new RouteValueDictionary { { "style", "display:none" }, { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                : htmlHelper.HiddenFor(expression, new RouteValueDictionary { { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                ) + htmlHelper.TextBox(name + "_hidden", value, newAttributes).ToString() +
                getScripts<M, T>(
                htmlHelper,
                name,
                watermarkCss,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                overrideWatermark, calendarOptions,
                ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }

        public static MvcHtmlString TypedTextBoxFor<M, T>(this HtmlHelper<M> htmlHelper,
            Expression<Func<M, T>> expression,
            string watermarkCss = null,
            ContentAlign contentAlign = ContentAlign.None,
            string overrideClientFormat = null,
            string overrideClientPrefix = null,
            string overrideClientPostfix = null,
            string overrideWatermark = null,
            CalendarOptions calendarOptions = null)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            IDictionary<string, object> newAttributes;
            applyAlignement(typeof(T), contentAlign, null, out newAttributes);
            if (watermarkCss == null) watermarkCss = "Theme-TypedTextBox-Watermark";
            string name = ExpressionHelper.GetExpressionText(expression);
            object value = null;
            try
            {
                value = expression.Compile().Invoke(htmlHelper.ViewData.Model);
            }
            catch
            {
            }
            //applyFunctions(htmlHelper, name, newAttributes);
            return MvcHtmlString.Create(
                
                (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient ?
                htmlHelper.TextBoxFor(expression, new RouteValueDictionary { { "style", "display:none" }, { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                : htmlHelper.HiddenFor(expression, new RouteValueDictionary { { "data-companionpostfix", "_hidden" }, { "data-elementispart", "true" } }).ToString()
                ) + htmlHelper.TextBox(name + "_hidden", value, newAttributes).ToString() +
                getScripts<M, T>(
                htmlHelper,
                name,
                watermarkCss,
                overrideClientFormat,
                overrideClientPrefix,
                overrideClientPostfix,
                overrideWatermark, calendarOptions, 
                ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData)));
        }
        private static string buttonScript = @"
            <script language='javascript' type='text/javascript'>
                setTimeout(function(){{$('#{0}').trigger('click');}}, 0);
            </script>
            ";
        public static MvcHtmlString EditDisplayToggle<VM>(
            this HtmlHelper<VM> htmlHelper,
            string containerSelector,
            string gotoDisplayTextOrUrl,
            string gotoEditTextOrUrl,
            string id,
            IDictionary<string, object> htmlAttributes = null,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            string changeStateCallback = null
            )
        {
            if (string.IsNullOrWhiteSpace(containerSelector)) throw (new ArgumentNullException("containerSelector"));
            if (string.IsNullOrWhiteSpace(gotoDisplayTextOrUrl)) throw (new ArgumentNullException("gotoDisplayTextOrUrl"));
            if (string.IsNullOrWhiteSpace(gotoEditTextOrUrl)) throw (new ArgumentNullException("gotoEditTextOrUrl"));
            if (string.IsNullOrWhiteSpace(id)) throw (new ArgumentNullException("gotoEditTextOrUrl"));
            string buttonId = BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, id));
            return
                MvcHtmlString.Create(
                    htmlHelper.ManipulationButton(ManipulationButtonType.Custom,
                    gotoDisplayTextOrUrl,
                    string.Format("function(){{MvcControlsToolkit_EditDisplayButton('#{4}', '{0}', '{1}', '{2}', {3});}}",
                    containerSelector,
                    gotoDisplayTextOrUrl,
                    gotoEditTextOrUrl,
                    changeStateCallback == null ? "null" : changeStateCallback,
                    buttonId),
                    id,
                    manipulationButtonStyle,
                    htmlAttributes).ToString() +
                    string.Format(buttonScript, buttonId)
                );
        }
        public static MvcHtmlString EditDisplayToggle<VM>(
            this HtmlHelper<VM> htmlHelper,
            string containerSelector,
            string gotoDisplayTextOrUrl,
            string gotoEditTextOrUrl,
            string id,
            object htmlAttributes,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            string changeStateCallback = null
            )
        {
            return htmlHelper.EditDisplayToggle(
                    containerSelector,
                    gotoDisplayTextOrUrl,
                    gotoEditTextOrUrl,
                    id,
                    htmlAttributes == null ? null : new RouteValueDictionary(htmlAttributes),
                    manipulationButtonStyle,
                    changeStateCallback
                );
        }
    }
}

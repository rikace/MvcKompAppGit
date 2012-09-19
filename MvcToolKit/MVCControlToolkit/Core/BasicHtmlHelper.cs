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
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.DataAnnotations;
using MVCControlsToolkit.Controls;
using System.Web.Routing;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;
using System.Web;

namespace MVCControlsToolkit.Core
{
    internal class MvcCTJsonDateConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            //Define the ListItemCollection as a supported type.
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(DateTime), typeof(DateTime?) })); }
        }
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null) return null;
            object val = dictionary["DateInTicks"];
            if (val == null) return null;
            else return new DateTime((long)val);
            
        }
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            IDictionary<string, object> res = new Dictionary<string, object>();
            if (obj == null)
                res.Add("DateInTicks", null);
            else
            res.Add("DateInTicks",  ((DateTime)obj).Ticks);
            return res;
        }
    }
    public class BasicHtmlHelper
    {
        private static string trueFieldScript =
           @"
            
            <script language='javascript' type='text/javascript'>
              var {0}_True = '{1}';
            </script>   
            ";
        private static string formatFieldScript =
           @"
            
            <script language='javascript' type='text/javascript'>
              var {0}_Format = '{1}';
            </script>   
            ";
        public static string ClientEncode(object model)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter> { new MvcCTJsonDateConverter() });
            return serializer.Serialize(model);
        }
        public static object ClientDecode(string jSonModel, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(jSonModel)) return null;
           
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new List<JavaScriptConverter> {new MvcCTJsonDateConverter()});
            object result = serializer.Deserialize(jSonModel, targetType);
            return result;
        }
        public static MvcHtmlString TrueValue<VM>(HtmlHelper<VM> htmlHelper, string prefix, object value)
        {
            string id = IdFromName(prefix);
            string res = string.Empty;
            if (value != null) res = htmlHelper.Encode(Convert.ToString(value));
            
            return MvcHtmlString.Create(string.Format(trueFieldScript, id, res));

        }
        public static MvcHtmlString FormattedValue<VM>(HtmlHelper<VM> htmlHelper, string prefix, object value)
        {
            string id = IdFromName(prefix);
            string res = string.Empty;
            if (value != null) htmlHelper.Encode(Convert.ToString(value));

            return MvcHtmlString.Create(string.Format(formatFieldScript, id, res));

        }
        public static string FormattedValueString<VM>(HtmlHelper<VM> htmlHelper, string prefix, object value)
        {
            string id = IdFromName(prefix);
            string res = Convert.ToString(value);

            return string.Format(formatFieldScript, id, res);

        }
        static public void GetContainerTags(
            MVCControlsToolkit.Controls.ExternalContainerType externalContainerType,
            IDictionary<string, object> htmlattributes,
            out string openTag,
            out string closureTag)
        {
            switch (externalContainerType)
            {
                case MVCControlsToolkit.Controls.ExternalContainerType.div:
                    openTag = string.Format("<div {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</div>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.span:
                    openTag = string.Format("<span {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</span>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.table:
                    openTag = string.Format("<table {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</table>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.tr:
                    openTag = string.Format("<tr {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</tr>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.td:
                    openTag = string.Format("<td {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</td>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.th:
                    openTag = string.Format("<th {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</th>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.ul:
                    openTag = string.Format("<ul {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</ul>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.ol:
                    openTag = string.Format("<ol {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</ol>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.li:
                    openTag = string.Format("<li {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</li>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.section:
                    openTag = string.Format("<section {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</section>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.article:
                    openTag = string.Format("<article {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</article>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.tbody:
                    openTag = string.Format("<tbody {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</tbody>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.thead:
                    openTag = string.Format("<thead {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</thead>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.tfoot:
                    openTag = string.Format("<tfoot {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</tfoot>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.menu:
                    openTag = string.Format("<menu {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</menu>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.nav:
                    openTag = string.Format("<nav {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</nav>";
                    break;
                case MVCControlsToolkit.Controls.ExternalContainerType.koComment:
                    string attrs=htmlattributes["data-bind"] as string;
                    if (attrs != null) attrs = attrs.Replace("&quot;", "'");
                    else attrs=string.Empty;
                    openTag = string.Format("<!-- ko {0} -->", attrs);
                    closureTag = "<!-- /ko -->";
                    break;
                default:
                    openTag = string.Format("<p {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</p>";
                    break;

            }
            return;
        }
        public static string GetAttributesString(object x)
        {
            if (x == null) return string.Empty;
            Type type = x.GetType();
            PropertyInfo[] properties = type.GetProperties();
            if (properties == null || properties.Length == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(x, new object[0]);
                if (value != null) sb.Append(string.Format(CultureInfo.InvariantCulture,
                    " {0}=\"{1}\" ", property.Name, value));
            }
            return sb.ToString();
        }
        public static string GetAttributesString(IDictionary<string, object> x)
        {
            if (x == null) return string.Empty;
            
            StringBuilder sb = new StringBuilder();
            foreach (string key in x.Keys)
            {
                object value = x[key];
                if (value != null) sb.Append(string.Format(CultureInfo.InvariantCulture,
                    " {0}=\"{1}\" ", key, Convert.ToString(value)));
            }
            return sb.ToString();
        }
        public static string TranslateAnonymous(object x)
        {
            if (x == null) return string.Empty;
            Type type = x.GetType();
            PropertyInfo[] properties = type.GetProperties();
            if (properties == null || properties.Length == 0) return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(x, new object[0]);
                if (sb.Length > 0) sb.Append(", ");
                if (value != null) sb.Append(string.Format(CultureInfo.InvariantCulture,
                    "{0}: {1}", property.Name, value));
            }
            return sb.ToString();
        }
        public static void SetDefaultStyle(IDictionary<string, object> htmlAttributes, string styleProperty, string stylePropertyValue)
        {

            
            if (htmlAttributes.ContainsKey("style"))
            {
                string prevStyle = htmlAttributes["style"].ToString();
                if (prevStyle.Contains(styleProperty)) return;
                else htmlAttributes["style"] = string.Format("{0} {1}: {2};", prevStyle, styleProperty, stylePropertyValue);
            }
            else
                htmlAttributes.Add("style", string.Format("{0}: {1};", styleProperty, stylePropertyValue));
        }
        public static void SetDefaultAttribute(IDictionary<string, object> htmlAttributes, string attributeName, string attributeValue)
        {

            if (htmlAttributes.ContainsKey(attributeName))
            {
                string prevAttribute = htmlAttributes[attributeName].ToString();
                if (!string.IsNullOrWhiteSpace(prevAttribute)) return;
                else htmlAttributes[attributeName] = attributeValue;
            }
            else
                htmlAttributes.Add(attributeName, attributeValue);
        }
        public static void SetAttribute(IDictionary<string, object> htmlAttributes, string attributeName, string attributeValue)
        {
            if (attributeValue == null) htmlAttributes.Remove(attributeName);
            else htmlAttributes[attributeName] = attributeValue;
        }
        public static RouteValueDictionary AnonymousObjectToDictionary(object htmlAttributes)
        {
            RouteValueDictionary result = new RouteValueDictionary();

            if (htmlAttributes != null)
            {
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(htmlAttributes))
                {
                    result.Add(property.Name.Replace('_', '-'), property.GetValue(htmlAttributes));
                }
            }

            return result;
        }
        public static void AddClass(IDictionary<string, object> htmlAttributes, string className)
        {

            if (htmlAttributes.ContainsKey("class"))
            {
                string prevClasses = htmlAttributes["class"].ToString();
                if (!string.IsNullOrWhiteSpace(prevClasses)) htmlAttributes["class"] = prevClasses + " " + className;
                else htmlAttributes["class"] = className;
            }
            else
                htmlAttributes.Add("class", className);
        }
        public static IDictionary<string, string> EncodeStyle(string styleString)
        {
            IDictionary<string, string> res = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(styleString)) return res;
            string[] propertyes = styleString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string property in propertyes)
            {
                string[] propertyValue=property.Split(':');
                res[propertyValue[0]] = propertyValue[1];
            }
            return res;
        }
        public static string DecodeStyle(IDictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            bool hasBefore=false;
            foreach (KeyValuePair<string, string> couple in dict)
            {
                if (hasBefore) sb.Append("; ");
                sb.Append(couple.Key);
                sb.Append(": ");
                sb.Append(couple.Value);
            }
            return sb.ToString();
        }
        public static string SetStyle(IDictionary<string, object> htmlAttributes, string styleProperty, string value)
        {
            string prevStyle = null;
            if (htmlAttributes.ContainsKey("style"))
            {
                prevStyle = htmlAttributes["style"].ToString();
                if (prevStyle.Contains(styleProperty))
                {
                    IDictionary<string, string> styleDict = EncodeStyle(prevStyle);
                    styleDict[styleProperty] = value;
                    htmlAttributes["style"] = DecodeStyle(styleDict);
                }
                else htmlAttributes["style"] = string.Format("{0} {1}: {2};", prevStyle, styleProperty, value);
            }
            else
                htmlAttributes.Add("style", string.Format("{0}: {1};", styleProperty, value));
            return
                prevStyle;
        }
        static internal string GetUniqueSymbol(HtmlHelper htmlHelper, string template)
        {
            int index = 0;
            if (htmlHelper.ViewData.ContainsKey(template))
            {
                index = (int)(htmlHelper.ViewData[template]);

            }
            htmlHelper.ViewData[template] = index + 1;
            return string.Format(template, index);
        }
        static public string RenderDisplayInfo(HtmlHelper htmlHelper, Type displayType, string prefix, bool duplicate=false)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                if (string.IsNullOrWhiteSpace(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix))
                    prefix = "display.$$";
                else
                    prefix = "$$";
            }
            else
                prefix=prefix + ".$$";
            string key = prefix + "_D";
            if (htmlHelper.ViewData.ContainsKey(key))
            {
                if (duplicate)
                {
                    int count = (int)htmlHelper.ViewData[key];
                    count++;
                    htmlHelper.ViewData[key] = count;
                    return SafeHiddenU(htmlHelper, prefix,
                        displayType, count).ToString();
                }
                else return string.Empty;
            }
            else
                htmlHelper.ViewData[key] = 0;
            return SafeHiddenU(htmlHelper, prefix,
                    displayType).ToString();
        }
        static internal Type DecodePageType(string raw, IValueProvider vp)
        {
            int index = -1;
            raw=raw.Trim();
            if (!int.TryParse(raw, out index) || index < 0) return null;
            string name = ExDefaultBinder.TypeRegisterPrefix + raw;
            ValueProviderResult attemptedTransformer = vp.GetValue(name);
            if (attemptedTransformer != null && attemptedTransformer.AttemptedValue != null)
                return Type.GetType(attemptedTransformer.AttemptedValue, false);
            else   return null;

        }
        static internal Type DecodeDisplayInfo(string raw, IValueProvider vp)
        {
            Type res = DecodePageType(raw, vp);
            if (res == null) return Type.GetType(raw, false);
            else return res;
        }
        static internal string DecodeFieldsInfo(string raw)
        {
            return raw;
        }
        static internal Type DecodeUpdateInfo(string raw, Type previousType, IValueProvider vp)
        {
            if (raw.Trim().Length==0) return previousType;
            Type res = DecodePageType(raw, vp);
            if (res == null) return Type.GetType(raw, false);
            else return res;
        }
        static internal string RenderUpdateInfo<M>(HtmlHelper htmlHelper, IUpdateModel updateModel, ref string prefix, string[] expressions, string template = null, Type typeToUse=null)
        {
            return RenderUpdateInfoI(htmlHelper, updateModel, ref prefix, expressions, template, typeToUse);
        }
        private class UpdateInfo
        {
            public int CurrIndex;
            public Type CurrType;
        }
        static internal string RenderUpdateInfoI(HtmlHelper htmlHelper, IUpdateModel updateModel, ref string prefix, string[] expressions, string template=null, Type typeToUse=null, bool noOutput=false)
        {
            bool prefixEmpty = false;
            bool sameType = false;
            if (typeToUse == null) typeToUse = updateModel.GetType();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                if (string.IsNullOrWhiteSpace(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix))
                {
                    prefixEmpty = true;
                    prefix = ".$$";
                }
                else
                    prefix =  "$$";
            }
            else
                prefix = prefix + ".$$";
            if (template != null)
            {
                prefix = prefix + template;
            }
            else
            {
                int index = 0;
                if (htmlHelper.ViewData.ContainsKey(prefix))
                {
                    UpdateInfo infos = (htmlHelper.ViewData[prefix]) as UpdateInfo;
                    index = infos.CurrIndex;
                    sameType = infos.CurrType == typeToUse;
                }
                htmlHelper.ViewData[prefix] = new UpdateInfo { CurrIndex = index + 1, CurrType = typeToUse };
                prefix = prefix + index.ToString();
            }
            if (noOutput) return null;
            StringBuilder sb= new StringBuilder();
            foreach(string expr in expressions)
            {
                if(sb.Length!=0) sb.Append(",");
                sb.Append(expr);
            }
            string res = null;

            if (sameType)
            {
                res = SafeHiddenF(htmlHelper, (prefixEmpty ? "updatemodel" + prefix : prefix),
                        string.Empty).ToString()
                             +
                             SafeHiddenF(htmlHelper, (prefixEmpty ? "updatemodel" + prefix : prefix) + ".f$ields",
                             sb.ToString()).ToString();
            }
            else
            {
                res = SafeHiddenU(htmlHelper, (prefixEmpty ? "updatemodel" + prefix : prefix),
                        typeToUse).ToString()
                             +
                             SafeHiddenF(htmlHelper, (prefixEmpty ? "updatemodel" + prefix : prefix) + ".f$ields",
                             sb.ToString()).ToString();
            }
            
            

            return res;
        }
        public static void CopyRelevantErrors(ModelStateDictionary destination, ModelStateDictionary origin, string prefix)
        {
            foreach (KeyValuePair<string, ModelState> pair in origin)
            {
                if (pair.Key.StartsWith(prefix)) destination.Add(pair);
            }
        }
        public static void CopyRelevantNonEmptyErrors(ModelStateDictionary destination, ModelStateDictionary origin, string prefix)
        {
            foreach (KeyValuePair<string, ModelState> pair in origin)
            {
                if (pair.Key.StartsWith(prefix) && pair.Value.Errors.Count>0) destination.Add(pair);
            }
        }
        

        public static void ClearRelevantErrors(ModelStateDictionary origin, string prefix)
        {
            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, ModelState> pair in origin)
            {
                if (pair.Key.StartsWith(prefix)) toRemove.Add(pair.Key);
            }
            foreach (string x in toRemove) origin.Remove(x);
        }
        public static string AddField(string prefix, string field)
        {
            if (string.IsNullOrWhiteSpace(prefix)) return field;
            else if (string.IsNullOrWhiteSpace(field)) return prefix;
            else return prefix + "." + field;
        }
        public static string IdFromName(string htmlName)
        {
            return htmlName.Replace('$', '_').Replace('.', '_').Replace('[', '_').Replace(']', '_');
        }
        public static string LocalTemplateName<M>(HtmlHelper<M> htmlHelper, string globalName)
        {
            string normalizedTemplate = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Trim() + ".";
            string normalizedGlobalName = globalName.Trim();
            if (normalizedGlobalName.StartsWith(normalizedTemplate)) 
                return normalizedGlobalName.Substring(normalizedTemplate.Length);
            return
                normalizedGlobalName;
        }
        public static string PreviousPrefix(string prefix)
        {
            
            string normalizedPrefix = prefix.Trim();
            int lastDot = prefix.LastIndexOf('.');
            if (lastDot < 0) return string.Empty;
            else return normalizedPrefix.Substring(0, lastDot);
            
        }
        public static string JavaScriptDate(DateTime? date, bool clientTimezone=false)
        {
            if ((date == null) || !date.HasValue) return "null";
            if (clientTimezone)
            {
                date=date.Value.ToUniversalTime();
                return string.Format("new Date(Date.UTC({0}, {1}, {2}, {3}, {4}, {5}))", 
                date.Value.Year, date.Value.Month-1, date.Value.Day,
                date.Value.Hour, date.Value.Minute, date.Value.Second);
            }
            else
            {
                return string.Format("new Date({0}, {1}, {2}, {3}, {4}, {5})", 
                date.Value.Year, date.Value.Month-1, date.Value.Day,
                date.Value.Hour, date.Value.Minute, date.Value.Second);
            }      
        }
        public static object ClientValidationDate(object dateObject, bool clientTimezone = false)
        {
            if (dateObject == null) return null;
            DateTime? date = dateObject as DateTime?;
            if (clientTimezone)
            {
                date = date.Value.ToUniversalTime();
                return string.Format("/Date(Date.UTC({0}, {1}, {2}, {3}, {4}, {5}))/",
                date.Value.Year, date.Value.Month - 1, date.Value.Day,
                date.Value.Hour, date.Value.Minute, date.Value.Second);
            }
            else
            {
                return string.Format("/Date({0}, {1}, {2}, {3}, {4}, {5})/",
                date.Value.Year, date.Value.Month - 1, date.Value.Day,
                date.Value.Hour, date.Value.Minute, date.Value.Second);
            }
        }
        public static MvcHtmlString SafeHidden<T>(HtmlHelper<T> htmlHelper, string field, object value)
        {
            string sVal = htmlHelper.Encode(Convert.ToString(value));
            field = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(field);

            return MvcHtmlString.Create(
                    string.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                        BasicHtmlHelper.IdFromName(field),
                        field, sVal));
        }
        public static MvcHtmlString SafeHiddenU(HtmlHelper htmlHelper, string field, Type type, int id = 0)
        {


            int index = TransformationContext.AddType(type, htmlHelper);
            if (index <0)
            {
                object value = type.AssemblyQualifiedName;
                string sVal = htmlHelper.Encode(Convert.ToString(value));
                field = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(field);

                return MvcHtmlString.Create(
                        string.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                        id == 0 ? BasicHtmlHelper.IdFromName(field) : BasicHtmlHelper.IdFromName(field) + id.ToString(),
                            field, sVal));
            }
            else
            {
                string sindex=index.ToString(CultureInfo.InvariantCulture);
                field = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(field);
                return MvcHtmlString.Create(
                    string.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                    id == 0 ? BasicHtmlHelper.IdFromName(field): BasicHtmlHelper.IdFromName(field)+id.ToString(),
                        field, sindex));
            }
            
        }
        public static MvcHtmlString SafeHiddenF(HtmlHelper htmlHelper, string field, object value, int id = 0)
        {
            string sVal = htmlHelper.Encode(Convert.ToString(value));
            field = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(field);

            return MvcHtmlString.Create(
                    string.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                    id == 0 ? BasicHtmlHelper.IdFromName(field) : BasicHtmlHelper.IdFromName(field) + id.ToString(),
                        field, sVal));
        }/*
        public static MvcHtmlString SafeHiddenU(HtmlHelper htmlHelper, string field, Type type, int id = 0)
        {
            object value = type.AssemblyQualifiedName;
            string sVal = htmlHelper.Encode(Convert.ToString(value));
            field = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(field);

            return MvcHtmlString.Create(
                    string.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                    id == 0 ? BasicHtmlHelper.IdFromName(field) : BasicHtmlHelper.IdFromName(field) + id.ToString(),
                        field, sVal));
        }*/
        public static MvcHtmlString SafeHiddenUC(HtmlHelper htmlHelper, string field, object value, string id = null)
        {
            string sVal = htmlHelper.Encode(Convert.ToString(value));
            field = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(field);

            return MvcHtmlString.Create(
                    string.Format("<input type='hidden' id='{0}' name='{1}' value='{2}' />",
                    id == null ? BasicHtmlHelper.IdFromName(field) : id,
                        field, sVal));
        }
        public static PropertyInfo[] GetPropertiesForInput(Type t)
        {
            List<PropertyInfo> pRes = new List<PropertyInfo>();
            BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance  | BindingFlags.GetProperty;;
            
            foreach (PropertyInfo prop in t.GetProperties(bindings))
            {
                PropertyAccessor pa = new PropertyAccessor(prop.Name, t);
                MileStoneAttribute[] mas=pa[typeof(MileStoneAttribute)] as MileStoneAttribute[];
                ReadOnlyAttribute[] ros = pa[typeof(ReadOnlyAttribute)] as ReadOnlyAttribute[];
                if (!prop.CanWrite && typeof(IConvertible).IsAssignableFrom(prop.PropertyType)) continue;
                if ((mas == null || mas.Length == 0) && (ros == null || ros.Length == 0)) pRes.Add(prop);
            }
            return pRes.ToArray<PropertyInfo>();
        }
        internal static string GetStandardValue(object toFormat)
        {
            if (toFormat == null) return string.Empty;
            string value = null;
            Type TargetType = toFormat.GetType();

            if (TargetType == typeof(Int32))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<Int32>))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Int16))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<Int16>))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Int64))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<Int64>))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(UInt32))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<UInt32>))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(UInt16))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<UInt16>))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(UInt64))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<UInt64>))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(byte))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<byte>))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(sbyte))
            {
                value = string.Format("{0:d}", toFormat);
            }
            else if (TargetType == typeof(Nullable<sbyte>))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(decimal))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(Nullable<decimal>))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(float))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(Nullable<float>))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(double))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(Nullable<double>))
            {
                value = string.Format("{0:n}", toFormat);
            }
            else if (TargetType == typeof(DateTime))
            {
                value = string.Format("{0:s}", toFormat);
            }
            else if (TargetType == typeof(Nullable<DateTime>))
            {
                value = string.Format("{0:s}", toFormat);
            }
            else 
            {
                value = Convert.ToString(toFormat);
            }
            
            return value;
        }
    }
}

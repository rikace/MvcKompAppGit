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
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controller;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Reflection;


namespace MVCControlsToolkit.Controls.DataGrid
{
    public class DataGridItem<TItem,TModel> : IUpdateModel, IUpdateModelState, IUpdateModelType
         where TItem : class, new()
    {
        private static string templateSymbol = "QQS_23459_8645_ZUQ";
        public Tracker<TItem> Item { get; set; }
        private PropertyInfo[] propertiesToStore;
        private List<LambdaExpression> fieldsToStore=null;
        private string status = null;
        protected Type FatherType, ListType, TrackerType;
        internal void SetStatus(string status)
        {
            this.status = status;
        }
        internal void SetPropertiesToStore(PropertyInfo[] properties)
        {
            propertiesToStore = properties;
        }
        internal void SetFieldsToStore(FieldsToTrack fields)
        {
            if(fields!=null)
                fieldsToStore = fields.Fields;
        }
        public void SendFatherType(Type t)
        {
            FatherType = t;
            Type[] gArgs = FatherType.GetGenericArguments();
            if (gArgs.Length == 0)
            {
                TrackerType = typeof(object);
                ListType = typeof(object);
            }
            else
            {
                TrackerType = gArgs[0];
                ListType = TrackerType.GetGenericArguments()[0];
            }
            if (!ListType.IsAssignableFrom(typeof(TItem)))
                throw new NotSupportedException(string.Format(
                    ControlsResources.NotCompatibleTypes,
                    typeof(TItem).FullName,
                    ListType.FullName));
        }
        public object UpdateModel(object model, string[] fields)
        {
            
            if (!Item.Changed)
            {
                if (Item.OldValue == null) return model;
                Item.Value = Item.OldValue;
            }
            if (model == null)
            {

                Type allListType = typeof(List<string>).GetGenericTypeDefinition().MakeGenericType(TrackerType);
                model = allListType.GetConstructor(new Type[0]).Invoke(new object[0]);
            }
            object toAdd = Item;
            if (ListType != typeof(TItem) && TrackerType != typeof(object))
            {

                toAdd = TrackerType.GetConstructor(
                    new Type[]{typeof(object), typeof(object), typeof(bool)})
                    .Invoke(new object[]{Item.OldValue, Item.Value, Item.Changed});
                
            }
            (model as IList).Add(toAdd);
            return model;
        }

        public void ImportFromModel(object model, object[] fields, string[] fieldNams, object[] args = null)
        {
            if (model != null)
            {
                try
                {
                    Item = (Tracker<TItem>)model;
                    
                    
                }
                catch { }
            }
            
        }
        private void addIEnumerablePropertiesTorender(
            string prefix,
            HtmlHelper<TModel> htmlHelper,
            StringBuilder sb,
            IEnumerable en,
            Stack<object> recursionControl)
        {
            int i=0;
            foreach (object obj in en)
            {
                if (obj == null) {
                    i++;
                    continue;
                }

                if (obj is IEnumerable)
                {
                    bool recursion = false;
                    foreach (object ob in recursionControl)
                    {
                        if (object.ReferenceEquals(obj, ob)) recursion=true;
                    }
                    if (recursion) continue;
                    recursionControl.Push(obj);
                    addIEnumerablePropertiesTorender(
                        string.Format("{0}[{1}]", prefix, i),
                        htmlHelper,
                        sb,
                        obj as IEnumerable,
                        recursionControl);
                    recursionControl.Pop();
                }
                else if (obj is IConvertible)
                {
                    string res = BasicHtmlHelper.SafeHidden(htmlHelper,
                        string.Format("{0}[{1}]", prefix, i), obj).ToString();
                    sb.Append(res);
                    i++;
                }
                else
                {
                    bool recursion = false;
                    foreach (object ob in recursionControl)
                    {
                        if (object.ReferenceEquals(obj, ob)) recursion = true;
                    }
                    if (recursion) continue;
                    recursionControl.Push(obj);
                    addObjectPropertiesTorender(
                    string.Format("{0}[{1}]", prefix, i),
                    htmlHelper,
                    sb,
                    BasicHtmlHelper.GetPropertiesForInput(obj.GetType()),
                    obj,
                    recursionControl);
                    recursionControl.Pop();
                }

            }
        }
        private void addObjectPropertiesTorender(string prefix,
            HtmlHelper<TModel> htmlHelper,
            StringBuilder sb,
            PropertyInfo[] propertiesToStore,
            object obj,
            Stack<object> recursionControl)
        {
            foreach (PropertyInfo pi in propertiesToStore)
            {
                if (pi.GetIndexParameters().GetLength(0) > 0) continue;
                object val = pi.GetValue(obj, new object[0]);
                if (val is IConvertible)
                {
                    string res = BasicHtmlHelper.SafeHidden(htmlHelper,
                        BasicHtmlHelper.AddField(prefix, pi.Name), val).ToString();
                    sb.Append(res);
                }
                else if (val is IEnumerable)
                {
                    bool recursion = false;
                    foreach (object ob in recursionControl)
                    {
                        if (object.ReferenceEquals(val, ob)) recursion = true; ;
                    }
                    if (recursion) continue;
                    recursionControl.Push(val);
                    addIEnumerablePropertiesTorender(
                        BasicHtmlHelper.AddField(prefix, pi.Name),
                        htmlHelper,
                        sb,
                        val as IEnumerable,
                        recursionControl);
                    recursionControl.Pop();
                }
                else if (val != null)
                {
                    bool recursion = false;
                    foreach (object ob in recursionControl)
                    {
                        if (object.ReferenceEquals(val, ob)) recursion = true ;
                    }
                    if (recursion) continue;
                    recursionControl.Push(val);
                    addObjectPropertiesTorender(
                        BasicHtmlHelper.AddField(prefix, pi.Name),
                        htmlHelper,
                        sb,
                        BasicHtmlHelper.GetPropertiesForInput(val.GetType()), val, recursionControl);
                    recursionControl.Pop();

                }

            }
        }
        internal string GetChangedAndOldValuesRendering(string prefix, HtmlHelper<TModel> htmlHelper)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append(BasicHtmlHelper.SafeHidden(htmlHelper, BasicHtmlHelper.AddField(prefix, "$.Item.Changed"), this.Item.Changed).ToString());
            if (this.Item != null && this.Item.OldValue != null)
            {
                prefix = BasicHtmlHelper.AddField(prefix, "$.Item.OldValue");
                if (fieldsToStore != null)
                {
                    foreach (LambdaExpression l in fieldsToStore)
                    {
                        object val = null;
                        string name = ExpressionHelper.GetExpressionText(l);
                        try
                        {
                            val = l.Compile().DynamicInvoke(this.Item.OldValue);
                        }
                        catch
                        {
                        }
                        if (val is IConvertible || val is Guid)
                        {
                            string res = BasicHtmlHelper.SafeHidden(htmlHelper,
                                BasicHtmlHelper.AddField(prefix, name), val).ToString();
                            sb.Append(res);
                        }
                        else 
                        {
                            MVCControlsToolkit.Controls.Bindings.ModelTranslatorLight model = new MVCControlsToolkit.Controls.Bindings.ModelTranslatorLight();
                            model.ImportFromModel(val);
                            sb.Append(BasicHtmlHelper.RenderDisplayInfo(htmlHelper, typeof(MVCControlsToolkit.Controls.Bindings.ModelTranslatorLight), 
                                BasicHtmlHelper.AddField(prefix, name)));
                            string res = BasicHtmlHelper.SafeHidden(htmlHelper,
                                BasicHtmlHelper.AddField(prefix, name + ".$.JSonModel"), model.JSonModel).ToString();
                            sb.Append(res);
                        }
                    }
                }
                else
                {
                    Stack<object> recursionControl = new Stack<object>();
                    recursionControl.Push(this.Item.OldValue);
                    addObjectPropertiesTorender(prefix, htmlHelper, sb, propertiesToStore, this.Item.OldValue, recursionControl);
                    recursionControl.Pop();
                }
                
            }
            return sb.ToString();
        }
        protected string GetContentRendering(string prefix, HtmlHelper<TModel> htmlHelper, object template, bool displayOnly=false)
        {
            ViewDataDictionary<TItem> dataDictionary = null;
            dataDictionary = new ViewDataDictionary<TItem>(Item.Value);
            if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
            {
                dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"] as GridDescriptionBase;
            }
            if (displayOnly)
                dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item.Value");
            else
            {
                dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item.Value");
                BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            }
            dataDictionary["status"]=status;
            return new TemplateInvoker<TItem>(template).Invoke<TModel>(htmlHelper, dataDictionary);
        }
        protected string GetOldContentRendering(string prefix, HtmlHelper<TModel> htmlHelper, object template)
        {
            ViewDataDictionary<TItem> dataDictionary = new ViewDataDictionary<TItem>(Item.OldValue);
            if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
            {
                dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"] as GridDescriptionBase;
            }
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item.Value");
            BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            dataDictionary["status"] = status;
            return new TemplateInvoker<TItem>(template).Invoke<TModel>(htmlHelper, dataDictionary);
        }
        protected string GetOldContentRenderingToAdd(string prefix, HtmlHelper<TModel> htmlHelper, object template)
        {
            ViewDataDictionary<TItem> dataDictionary = new ViewDataDictionary<TItem>(Item.OldValue);
            if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
            {
                dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"] as GridDescriptionBase;
            }
            dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item.Value"+templateSymbol);
            //BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
            dataDictionary["status"] = status;
            string res = new TemplateInvoker<TItem>(template).Invoke<TModel>(htmlHelper, dataDictionary).Replace(templateSymbol, "");
            return res;
        }

        internal string GetEditItem(string prefix,
            HtmlHelper<TModel> htmlHelper,
            object editTemplate,
            ItemContainerType itemContainerType,
            IDictionary<string, object> htmlattributes,
            int index,
            bool normal,
            Func<TItem, int, string> getDynamicContainer = null
            )
        {
            string openTag;
            string closureTag;
            GetItemContainerTags(
                itemContainerType,
                index,
                getDynamicContainer,
                htmlattributes,
                BasicHtmlHelper.AddField(prefix, "$.Item.Value.Edit"),
                !normal,
                out openTag,
                out closureTag);
            return string.Format("{1} {0} {2}",
                GetContentRendering(prefix, htmlHelper, editTemplate),
                openTag,
                closureTag);
        }
        internal string GetOldEditItem(string prefix,
           HtmlHelper<TModel> htmlHelper,
           object editTemplate,
           ItemContainerType itemContainerType,
           IDictionary<string, object> htmlattributes,
           int index,
           bool normal,
           Func<TItem, int, string> getDynamicContainer = null
           )
        {
            string openTag;
            string closureTag;
            GetItemContainerTags(
                itemContainerType,
                index,
                getDynamicContainer,
                htmlattributes,
                BasicHtmlHelper.AddField(prefix, "$.Item.Value.OldEdit"),
                !normal,
                out openTag,
                out closureTag);
            return string.Format("{1} {0} {2}",
                GetOldContentRenderingToAdd(prefix, htmlHelper, editTemplate),
                openTag,
                closureTag);
        }
       internal string GetDisplayItem(string prefix,
            HtmlHelper<TModel> htmlHelper,
            object displayTemplate,
            ItemContainerType itemContainerType,
            IDictionary<string, object> htmlattributes,
            int index,
            bool normal,
            Func<TItem, int, string> getDynamicContainer = null
            )
        {
            string openTag;
            string closureTag;
            GetItemContainerTags(
                itemContainerType,
                index,
                getDynamicContainer,
                htmlattributes,
                BasicHtmlHelper.AddField(prefix, "$.Item.Value.Display"),
                !normal,
                out openTag,
                out closureTag);
            return string.Format("{1} {0} {2}",
                (Item.Changed ?
                    GetOldContentRendering(prefix, htmlHelper, displayTemplate) :
                    GetContentRendering(prefix, htmlHelper, displayTemplate, true)),
                openTag,
                closureTag);
        }
       internal string GetDeleteItem(string prefix,
           HtmlHelper<TModel> htmlHelper,
           object displayTemplate,
           ItemContainerType itemContainerType,
           IDictionary<string, object> htmlattributes,
           int index,
           bool normal,
           Func<TItem, int, string> getDynamicContainer = null
           )
       {
           string openTag;
           string closureTag;
           GetItemContainerTags(
               itemContainerType,
               index,
               getDynamicContainer,
               htmlattributes,
               BasicHtmlHelper.AddField(prefix, "$.Item.Value.Undelete"),
               !normal,
               out openTag,
               out closureTag);
           return string.Format("{1} {0} {2}",
               (Item.Changed ?
                   GetOldContentRendering(prefix, htmlHelper, displayTemplate) :
                   GetContentRendering(prefix, htmlHelper, displayTemplate)),
               openTag,
               closureTag);
       }
       internal string GetChangedItem(string prefix,
          HtmlHelper<TModel> htmlHelper,
          object displayTemplate,
          ItemContainerType itemContainerType,
          IDictionary<string, object> htmlattributes,
          int index,
          bool normal,
          Func<TItem, int, string> getDynamicContainer = null
          )
       {
           string openTag;
           string closureTag;
           GetItemContainerTags(
               itemContainerType,
               index,
               getDynamicContainer,
               htmlattributes,
               BasicHtmlHelper.AddField(prefix, "$.Item.Value.ChangedExternally"),
               !normal,
               out openTag,
               out closureTag);
           return string.Format("{1} {0} {2}",
               (Item.Changed ?
                   GetOldContentRendering(prefix, htmlHelper, displayTemplate) :
                   GetContentRendering(prefix, htmlHelper, displayTemplate)),
               openTag,
               closureTag);
       }
        protected void GetItemContainerTags(
            ItemContainerType itemContainerType,
            int index,
            Func<TItem, int, string> getDynamicContainer,
            IDictionary<string, object> htmlattributes,
            string prefix,
            bool hidden,
            out string openTag,
            out string closureTag
            )
        {
            if (htmlattributes == null) htmlattributes = new Dictionary<string, object>();
            BasicHtmlHelper.SetAttribute(htmlattributes, "id", BasicHtmlHelper.IdFromName(prefix) + "_Container");
            
            if (hidden)
            {
                
                BasicHtmlHelper.SetDefaultStyle(htmlattributes, "display", "none");
            }
            switch (itemContainerType)
            {
                case ItemContainerType.div:
                    openTag = string.Format("<div {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</div>";
                    break;
                case ItemContainerType.span:
                    openTag = string.Format("<span {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</span>";
                    break;
                case ItemContainerType.tr:
                    openTag = string.Format("<tr {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</tr>";
                    break;
                case ItemContainerType.td:
                    openTag = string.Format("<td {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</td>";
                    break;
                case ItemContainerType.li:
                    openTag = string.Format("<li {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</li>";
                    break;
                case ItemContainerType.section:
                    openTag = string.Format("<section {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</section>";
                    break;
                case ItemContainerType.article:
                    openTag = string.Format("<article {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</article>";
                    break;

                case ItemContainerType.p:
                    openTag = string.Format("<p {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                    closureTag = "</p>";
                    break;
                default:
                    if (getDynamicContainer != null)
                    {
                        string tagName = getDynamicContainer(Item.OldValue, index);
                        openTag = string.Format("<{1} {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes), tagName);
                        closureTag = string.Format("</{0}>", tagName);
                    }
                    else
                    {
                        openTag = string.Format("<div {0}>", BasicHtmlHelper.GetAttributesString(htmlattributes));
                        closureTag = "</div>";
                    }
                    break;

            }
        }

        public void GetCurrState(string currPrefix, int updateIndex, ModelStateDictionary modelState)
        {
            string prefix = "$${0}.$.Item.OldValue";
            if (!string.IsNullOrWhiteSpace(currPrefix) && currPrefix != "display")
            {
                prefix = currPrefix + "." + prefix;
            }
            BasicHtmlHelper.ClearRelevantErrors(modelState, string.Format(prefix, updateIndex));
        }
        public bool MoveState { get { return false; } }
        
        
    }
}

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
using MVCControlsToolkit.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;


namespace MVCControlsToolkit.Controls
{
    public static class SortableListHelper
    {
        private static string templateSymbol = "QS_23459_86{0}45_ZU";
        private static string startScriptFormat =
        @"
            <script language='javascript' type='text/javascript'>
                var {0}_PermutationUpdateRoot = '{2}';
                $(document).ready(function()
                {{
                    $('#{0}_ItemsContainer' ).sortable({{ {1} start:function(e, ui){{$(document.activeElement).trigger('blur');}}, update: function(event, ui) {{MvcControlsToolkit_SortableListUpdate(ui.item, '{0}_ItemsContainer'); }} }});
                    
                }});
            </script>
        ";
        private static string addButtonScript = " MvcControlsToolkit_SortableList_AddNewChoice(\"{0}\", {1});";
        private static string stylingScript =
        @"
            <script language='javascript' type='text/javascript'>
                 var {0}_Css = '{1}';
                 var {0}_AltCss = '{2}';
                 $(document).ready(function()
                {{
                    Update_Permutations_Root('{0}');
                }});
            </script>
        ";
        private static string startTemplateScriptFormat =
       @"
            <script language='javascript' type='text/javascript'>
                 var {0}_CanSort = {1};
                 var {0}_ElementsNumber = {2};
                 var {0}_OriginalElementsNumber = {2};
                 var {0}_TemplateSymbol = [{3}];
                 var {0}_TemplateSript = null;
                 var {0}_TemplateHtml = null;
                 var {0}_TemplateHidden = [{5}];
                 var {0}_TemplateHiddenHtml = null;
                 var {0}_NamePrefix = '{6}';
                $(document).ready(function()
                {{
                   
                    MvcControlsToolkit_SortableList_PrepareTemplates('{0}', [{4}]); 
                }});
            
            </script>
        ";

       // private static string addItemLegacyScriptFormat =
       //"<script language='javascript' type='text/javascript'>" +
       // "       $(document).ready(function()" +
       // "        {{ " +
       // "            setTimeout(\"MvcControlsToolkit_SortableList_Click('{0}', 'ManipulationButtonHide');\", 0); " +
       // "        }});" +
       // "</script>";
        private static string addItemJQueryScriptFormat =

        "<script language='javascript' type='text/javascript'>" +
        "       $(document).ready(function()" +
        "        {{ " +
        "            setTimeout(\"MvcControlsToolkit_SortableList_Click('{0}', 'ManipulationButtonHide', true);\", 0); " +
        "        }});" +
        "</script>";

        public static MvcHtmlString ThemedSortableListFor<M, TItem>(
            this HtmlHelper<M> htmlHelper,
            Expression<Func<M, IEnumerable<TItem>>> expression,
            SortableListFeatures sortableListFeatures,
            Columns<TItem> fields,
            Expression<Func<M, IEnumerable<KeyValuePair<LambdaExpression, OrderType>>>> orderExpression = null,
            Expression<Func<M, int>> page = null,
            Expression<Func<M, int>> prevPage = null,
            Expression<Func<M, int>> pageCount = null,
            Expression<Func<M, Expression<Func<TItem, bool>>>> filter = null,
            string title = null,
            string name = "SortableList"
            )
            where TItem : class, new()
        {
            if (expression == null) throw (new ArgumentNullException("(expression"));
            
            if (fields == null || fields.Fields == null || fields.Fields.Count == 0) throw (new ArgumentNullException("fields"));
            string themeName = ThemedControlsStrings.GetTheme();
            htmlHelper.ViewData["ThemeParams"] =
                new SortableListDescription
                {
                    ToShow = expression,
                    ToOrder = orderExpression,
                    Fields = fields.Fields,
                    HtmlHelper = htmlHelper,
                    Features = sortableListFeatures,
                    Title = title,
                    Page = page,
                    PrevPage = prevPage,
                    PageCount = pageCount,
                    Filter = filter
                };
            MvcHtmlString res;
            try
            {
                res = htmlHelper.Partial("Themes/" + themeName + "/" +name, htmlHelper.ViewData);
            }
            finally
            {
                htmlHelper.ViewData["ThemeParams"] = null;
            }
            return res;

        }

        public static MvcHtmlString SortableListAddButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            string textOrUrl,
             ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl,
                                  string.Format("MvcControlsToolkit_SortableList_Click(\"{0}\", \"ManipulationButtonShow\")", htmlHelper.PrefixedId("InnerContainer")),
                                  htmlHelper.PrefixedId("Add"), manipulationButtonStyle, htmlAttributes);
        }
        public static MvcHtmlString SortableListAddButtonFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<T>>> expression,
            string textOrUrl,
             ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null,
            string name = null,
            int templateIndex=0)
        { 
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient) return MvcHtmlString.Create(string.Empty);
            string id = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl,
                                  string.Format(addButtonScript, id, templateIndex), name == null ? id + "_AddButton"+templateIndex.ToString(CultureInfo.InvariantCulture) : name, manipulationButtonStyle, htmlAttributes);
        }
        public static MvcHtmlString SortableListAddButtonFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<TItem>> renderInfo,
            string textOrUrl,
             ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null,
            string name=null,
            int templateIndex=0)
        {
            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));
            if (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient) return MvcHtmlString.Create(string.Empty);
            string id = BasicHtmlHelper.IdFromName(renderInfo.Prefix);
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl,
                string.Format(addButtonScript, id, templateIndex), name == null ? id + "_AddButton" + templateIndex.ToString(CultureInfo.InvariantCulture) : name, manipulationButtonStyle, htmlAttributes);
        }
        public static MvcHtmlString SortableListCancelButton<VM>(
           this HtmlHelper<VM> htmlHelper,
           string textOrUrl,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
           IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl, string.Format("MvcControlsToolkit_SortableList_Click(\"{0}\", \"ManipulationButtonHide\");", htmlHelper.PrefixedId("InnerContainer")),
                                  htmlHelper.PrefixedId("Cancel"), manipulationButtonStyle, htmlAttributes);
        }
        public static MvcHtmlString SortableListDeleteButton<VM>(
           this HtmlHelper<VM> htmlHelper,
           string textOrUrl,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
           IDictionary<string, object> htmlAttributes = null)
        {
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl, string.Format("MvcControlsToolkit_SortableList_Click(\"{0}\", \"ManipulationButtonRemove\");", htmlHelper.PrefixedId("Container")),
                                  htmlHelper.PrefixedId("Remove"), manipulationButtonStyle, htmlAttributes);
        }
        public static string SortableListNewName<VM>(this HtmlHelper<VM> htmlHelper)
        {
            return htmlHelper.PrefixedId("InnerContainer");
        }
        private static IDictionary<string, object> dictionarySelection(Func<int, IDictionary<string, object>> dd, IDictionary<string, object> sd, int index)
        {
            if (dd == null) return sd;
            IDictionary<string, object> res = dd(index);
            return res == null ? sd : res;
        }
        public static MvcHtmlString SortableListFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<TItem>> renderInfo,
            object template,
            object addElementTemplate = null,
            float opacity = 1,
            bool canSort = true,
            IDictionary<string, object> htmlAttributesContainer = null,
            IDictionary<string, object> htmlAttributesItems = null,
            bool enableMultipleInsert = true,
            ExternalContainerType itemContainer=ExternalContainerType.li,
            ExternalContainerType allItemsContainer=ExternalContainerType.ul,
            object headerTemplate = null,
            object footerTemplate = null,
            string itemCss=null, 
            string altItemCss=null,
            bool displayOnly = false,
            string sortableHandle = null, 
            Func<TItem, int> templateSelector=null,
            Func<int, IDictionary<string, object>> htmlAttributesSelector = null
            )
        {
            if (template == null) throw (new ArgumentNullException("template"));
            if (renderInfo == null) throw (new ArgumentNullException("renderiNFO"));
            if(string.IsNullOrWhiteSpace(itemCss)) itemCss = string.Empty;
            if(string.IsNullOrWhiteSpace(altItemCss)) altItemCss = string.Empty;
            if (displayOnly) canSort = false;
            if (canSort)
            {
                itemContainer = ExternalContainerType.li;
                allItemsContainer = ExternalContainerType.ul;
                headerTemplate = null;
                footerTemplate = null;
                htmlHelper.ViewData[renderInfo.Prefix + "_Rendering"] = "SortableList_Dragging";
            }
            else
            {
                htmlHelper.ViewData[renderInfo.Prefix + "_Rendering"] = "SortableList_Dragging";
            }
            bool multipleTemplates = false;
            if (template is string || !(template is object[]))
            {
                template = new object[] { template };
            }
            else if (templateSelector != null)
            {
                multipleTemplates = true;
            }
            int nTemplates = (template as object[]).Length;
            Type[] allTypes = new Type[nTemplates];
            int tcount = 0;
            foreach(object t in (template as object[]))
            {
                if (t is string) allTypes[tcount] = typeof(TItem);
                else allTypes[tcount] = TemplateInvoker<string>.ExtractModelType(t);
                tcount++;
            }
            StringBuilder sb = new StringBuilder();
            StringBuilder sbInit = new StringBuilder();

            sbInit.Append(renderInfo.PartialRendering);
            if(htmlAttributesContainer == null) htmlAttributesContainer=new Dictionary<string, object>();
            htmlAttributesContainer["id"]=BasicHtmlHelper.IdFromName(renderInfo.Prefix)+"_ItemsContainer";
            string externalOpenTag=null;
            string externalCloseTag = null;
            string itemOpenTag = null;
            string itemCloseTag = null;
            BasicHtmlHelper.GetContainerTags(allItemsContainer, htmlAttributesContainer, out externalOpenTag, out externalCloseTag);
            sb.Append(externalOpenTag);
            
            if (htmlAttributesItems == null)
            {
                htmlAttributesItems = new Dictionary<string, object>();
            }
            IDictionary<string, object> fixedHtmlAttributesItems = htmlAttributesItems;
            bool templateEnabled = false;
            string addItemScriptFormat = addItemJQueryScriptFormat;
            
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient:  break;
                case ValidationType.UnobtrusiveClient:  templateEnabled = true; break;
                default:  templateEnabled = true; break;
            }
             
            templateEnabled = templateEnabled && enableMultipleInsert ;
            int totalCount = 0;
            string javasctiptOpacity = string.Empty;
            string javascriptHandle = string.Empty;
            string permutationElementPrefix = null;
            if (renderInfo.Model == null) renderInfo.Model = new List<TItem>();
            if (renderInfo.Model != null)
            {
                if (headerTemplate != null)
                {
                    if (multipleTemplates) htmlAttributesItems = dictionarySelection(htmlAttributesSelector, fixedHtmlAttributesItems, -1);
                    ViewDataDictionary<TItem> dataDictionary = new ViewDataDictionary<TItem>();
                    dataDictionary.TemplateInfo.HtmlFieldPrefix = renderInfo.Prefix;
                    if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
                    {
                        dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"];
                    }
                    BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                    htmlAttributesItems["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(renderInfo.Prefix, "Header")) ;
                    BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesItems, out itemOpenTag, out itemCloseTag);
                    sb.Append(itemOpenTag);
                    sb.Append(new TemplateInvoker<TItem>(headerTemplate).Invoke<VM>(htmlHelper, dataDictionary) );
                    sb.Append(itemCloseTag);
                }
                foreach (TItem x in renderInfo.Model)
                {
                    if (x == null && htmlHelper.ViewContext.ViewData.ModelState.IsValid) continue;
                    totalCount++;
                    int templateIndex = 0;
                    Type currType = typeof(TItem);
                    if (multipleTemplates)
                    {
                        templateIndex = templateSelector(x);
                        currType = allTypes[templateIndex];
                        htmlAttributesItems = dictionarySelection(htmlAttributesSelector, fixedHtmlAttributesItems, templateIndex);
                    }
                    IUpdateModel um = null;
                    if (x == null)
                    {
                        um = typeof(AutoEnumerableUpdater<string>).GetGenericTypeDefinition().MakeGenericType(currType)
                        .GetConstructor(new Type[0]).Invoke(new object[0]) as IUpdateModel;
                    }
                    else if (x.GetType() == typeof(TItem))
                    {
                        um = new AutoEnumerableUpdater<TItem>();
                    }
                    else
                    {
                        um = typeof(AutoEnumerableUpdater<string>).GetGenericTypeDefinition().MakeGenericType(x.GetType())
                        .GetConstructor(new Type[0]).Invoke(new object[0]) as IUpdateModel;
                    }
                    um.ImportFromModel(x, null, null, new object[0]);
                    string prefix = renderInfo.Prefix;
                    string partialPrefix = renderInfo.PartialPrefix;
                    string updateInfo = BasicHtmlHelper.RenderUpdateInfo<TItem>(htmlHelper, um, ref partialPrefix, new string[0]);
                    if (!displayOnly) sbInit.Append(updateInfo );
                    prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                    object currTemplate = (template as object[])[templateIndex];

                    ViewDataDictionary dataDictionary = null;
                    ITemplateInvoker currInvoker = null;
                    if (allTypes[templateIndex] == typeof(TItem))
                    {
                        dataDictionary = new ViewDataDictionary<TItem>(x);
                        currInvoker = new TemplateInvoker<TItem>(currTemplate);
                    }
                    else
                    {
                        dataDictionary = typeof(ViewDataDictionary<string>).GetGenericTypeDefinition().MakeGenericType(allTypes[templateIndex])
                        .GetConstructor(new Type[] { allTypes[templateIndex] }).Invoke(new object[] { x }) as ViewDataDictionary;
                        
                        currInvoker = typeof(TemplateInvoker<string>).GetGenericTypeDefinition().MakeGenericType(allTypes[templateIndex])
                        .GetConstructor(new Type[] { typeof(object) }).Invoke(new object[] { currTemplate }) as ITemplateInvoker;
                    }
                    dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item");
                    if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
                    {
                        dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"];
                    }
                    BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                    htmlAttributesItems["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item"))+"_Container";
                    BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesItems, out itemOpenTag, out itemCloseTag);
                    sb.Append(itemOpenTag);
                    sb.Append(currInvoker.Invoke<VM>(htmlHelper, dataDictionary));
                    sb.Append(itemCloseTag);
                }
            }
            string delayedRendering = null;
            if (addElementTemplate != null && !templateEnabled)
            {
                EnumerableUpdater<TItem> um = new EnumerableUpdater<TItem>(true);
                string prefix = renderInfo.Prefix;
                string partialPrefix = renderInfo.PartialPrefix;
                sbInit.Append(
                    BasicHtmlHelper.RenderUpdateInfo<TItem>(htmlHelper, um, ref partialPrefix, new string[0]));
                prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                sbInit.Append(htmlHelper.GenericInput(InputType.Hidden,
                    BasicHtmlHelper.AddField(partialPrefix, "$.Deleted"), um.Deleted, null));
                delayedRendering=string.Format(
                    addItemScriptFormat,
                    BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.InnerContainer")));
                ViewDataDictionary<TItem> dataDictionary = new ViewDataDictionary<TItem>(um.Item);
                dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item");
                if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
                {
                    dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"];
                }
                BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                htmlAttributesItems["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item")) + "_Container";

                BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesItems, out itemOpenTag, out itemCloseTag);
                sb.Append(itemOpenTag);
                sb.Append(new TemplateInvoker<TItem>(addElementTemplate).Invoke<VM>(htmlHelper, dataDictionary));
                sb.Append(itemCloseTag);
            }
            if (canSort)
            {
                PermutationsUpdater<TItem> um = new PermutationsUpdater<TItem>();
                um.ImportFromModel(null, null, null, new object[0]);
                string prefix = renderInfo.Prefix;
                string partialPrefix = renderInfo.PartialPrefix;
                sbInit.Append(
                    BasicHtmlHelper.RenderUpdateInfo<TItem>(htmlHelper, um, ref partialPrefix, new string[0]));
                prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                sbInit.Append(
                    string.Format("<input type='hidden' id={0} name={1} value=''/>",
                        BasicHtmlHelper.IdFromName(renderInfo.Prefix+"_Permutation"),
                        BasicHtmlHelper.AddField(prefix, "$.Permutation")));
                if (opacity > 1f) opacity = 1f;
                else if (opacity < 0.01f) opacity = 0.01f;
                
                if (opacity < 1f)
                {
                    javasctiptOpacity=string.Format(" opacity: {0}, ", opacity.ToString(CultureInfo.InvariantCulture));
                }
                if (sortableHandle != null)
                {
                    javascriptHandle = string.Format(" handle: '{0}', ", sortableHandle);
                }
                permutationElementPrefix = prefix;

            }
            if (footerTemplate != null)
            {
                if (multipleTemplates) htmlAttributesItems = dictionarySelection(htmlAttributesSelector, fixedHtmlAttributesItems, -2);
                ViewDataDictionary<TItem> dataDictionary = new ViewDataDictionary<TItem>();
                dataDictionary.TemplateInfo.HtmlFieldPrefix = renderInfo.Prefix;
                if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
                {
                    dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"];
                }
                BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                htmlAttributesItems["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(renderInfo.Prefix, "Footer"));
                BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesItems, out itemOpenTag, out itemCloseTag);
                sb.Append(itemOpenTag);
                sb.Append(new TemplateInvoker<TItem>(footerTemplate).Invoke<VM>(htmlHelper, dataDictionary));
                sb.Append(itemCloseTag);
            }
            if (templateEnabled)
            {
                IUpdateModel um = null;
                TItem dummyItem = default(TItem);
                object[] allTemplates = template as object[];
                StringBuilder alltemplateNames = new StringBuilder();
                StringBuilder allhiddenNames = new StringBuilder();
                StringBuilder allsymbolNames = new StringBuilder();
                string basicTemplateSymbol = BasicHtmlHelper.GetUniqueSymbol(htmlHelper, templateSymbol);
                for (int i = 0; i < allTemplates.Length; i++)
                {
                    if (multipleTemplates) htmlAttributesItems = dictionarySelection(htmlAttributesSelector, fixedHtmlAttributesItems, i);
                    if (allTypes[i] == typeof(TItem))
                    {
                        um = new AutoEnumerableUpdater<TItem>();
                    }
                    else
                    {
                        um = typeof(AutoEnumerableUpdater<string>).GetGenericTypeDefinition().MakeGenericType(allTypes[i])
                        .GetConstructor(new Type[0]).Invoke(new object[0]) as IUpdateModel;
                    }
                    string prefix = renderInfo.Prefix;
                    string partialPrefix = renderInfo.PartialPrefix;
                    string myTemplateSymbol = basicTemplateSymbol+i.ToString(CultureInfo.InvariantCulture);
                 
                    sbInit.Append(
                        BasicHtmlHelper.RenderUpdateInfoI(htmlHelper, um, ref partialPrefix, new string[0], myTemplateSymbol));
                    prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);

                    ViewDataDictionary dataDictionary = null;
                    ITemplateInvoker currInvoker = null;
                    if (allTypes[i] == typeof(TItem))
                    {
                        dataDictionary = new ViewDataDictionary<TItem>(dummyItem);
                        currInvoker = new TemplateInvoker<TItem>(allTemplates[i]);
                    }
                    else
                    {
                        dataDictionary = typeof(ViewDataDictionary<string>).GetGenericTypeDefinition().MakeGenericType(allTypes[i])
                        .GetConstructor(new Type[] { allTypes[i] }).Invoke(new object[] { dummyItem }) as ViewDataDictionary;

                        currInvoker = typeof(TemplateInvoker<string>).GetGenericTypeDefinition().MakeGenericType(allTypes[i])
                        .GetConstructor(new Type[] { typeof(object) }).Invoke(new object[] { allTemplates[i] }) as ITemplateInvoker;
                    }

                    dataDictionary.TemplateInfo.HtmlFieldPrefix = BasicHtmlHelper.AddField(prefix, "$.Item");
                    if (htmlHelper.ViewData.ContainsKey("ThemeParams"))
                    {
                        dataDictionary["ThemeParams"] = htmlHelper.ViewData["ThemeParams"];
                    }
                    BasicHtmlHelper.CopyRelevantErrors(dataDictionary.ModelState, htmlHelper.ViewData.ModelState, dataDictionary.TemplateInfo.HtmlFieldPrefix);
                
                
                    string templateId = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item")) + "_Container";
                    htmlAttributesItems["id"] = templateId;
                    if (i > 0)
                    {
                        alltemplateNames.Append(", ");
                        allhiddenNames.Append(", ");
                        allsymbolNames.Append(", ");
                    }
                    alltemplateNames.Append("'");
                    alltemplateNames.Append(templateId);
                    alltemplateNames.Append("'");

                    allhiddenNames.Append("'");
                    allhiddenNames.Append(BasicHtmlHelper.IdFromName(prefix));
                    allhiddenNames.Append("'");

                    allsymbolNames.Append("/");
                    allsymbolNames.Append(myTemplateSymbol);
                    allsymbolNames.Append("/g");

                    BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesItems, out itemOpenTag, out itemCloseTag);
                    sb.Append(string.Format("<span id='{0}' style='display:none' class='MVCCT_EncodedTemplate'>", templateId));
                    sb.Append(htmlHelper.Encode(itemOpenTag));
                    sb.Append(htmlHelper.Encode(currInvoker.Invoke<VM>(htmlHelper, dataDictionary)));
                    sb.Append(htmlHelper.Encode(itemCloseTag));
                    sb.Append("</span>");
                }

                sbInit.Append(string.Format(startTemplateScriptFormat, BasicHtmlHelper.IdFromName(renderInfo.Prefix),
                    canSort ? "true" : "false", totalCount, allsymbolNames.ToString(), alltemplateNames.ToString(),
                    allhiddenNames.ToString(), renderInfo.Prefix));
            }

            if (canSort) sbInit.Append(string.Format(startScriptFormat, BasicHtmlHelper.IdFromName(renderInfo.Prefix), javascriptHandle+javasctiptOpacity, BasicHtmlHelper.IdFromName(permutationElementPrefix)));
            string stylingJavaScript = string.Format(stylingScript, BasicHtmlHelper.IdFromName(renderInfo.Prefix), itemCss, altItemCss);
            sbInit.Append(stylingJavaScript);
            if (delayedRendering != null) sbInit.Append(delayedRendering);
            sb.Append(externalCloseTag);
            sbInit.Insert(0, sb.ToString());
            return MvcHtmlString.Create(sbInit.ToString());
            
        }
        public static MvcHtmlString SortableListFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<TItem>>> expression,
            object template,
            object addElementTemplate = null,
            float opacity = 1,
            bool canSort = true,
            IDictionary<string, object> htmlAttributesContainer = null,
            IDictionary<string, object> htmlAttributesItems = null,
            bool enableMultipleInsert = true,
            ExternalContainerType itemContainer = ExternalContainerType.li,
            ExternalContainerType allItemsContainer = ExternalContainerType.ul,
            object headerTemplate = null,
            object footerTemplate = null,
            string itemCss = null,
            string altItemCss = null,
            bool displayOnly = false,
            string sortableHandle = null,
            Func<TItem, int> templateSelector = null,
            Func<int, IDictionary<string, object>> htmlAttributesSelector = null)
        {
            if (template == null) throw (new ArgumentNullException("template"));
            if (expression == null) throw (new ArgumentNullException("expression"));
            return SortableListFor(
                htmlHelper,
                htmlHelper.ExtractFromModel(expression),
                template,
                addElementTemplate,
                opacity,
                canSort,
                htmlAttributesContainer,
                htmlAttributesItems,
                enableMultipleInsert,
                itemContainer,
                allItemsContainer,
                headerTemplate,
                footerTemplate, 
                itemCss, 
                altItemCss,
                displayOnly,
                sortableHandle,
                templateSelector,
                htmlAttributesSelector);
        }
            
    }
}

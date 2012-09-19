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
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using MVCControlsToolkit.Controls.TreeView;


namespace MVCControlsToolkit.Controls
{
    public enum TreeViewItemStatus { initializeShow, InitializeHide, Show, Hide };
    public enum TreeViewMode {InitializeDisplay, InitializeEdit, Display, Edit };
    public static class TreeViewHelpers
    {
        private static string defaultRootClass = "filetree";
        private static string completeTreeScriptDisplay =
        @"
            <script language='javascript' type='text/javascript'>
                var {0}_SaveDisplay = null;
                var {0}_SaveEdit = null;
                var {0}_ButtonMode = 0; 
                {0}_SaveEdit = $('#{0}___Choice2___flattened_ItemsContainer');
                {0}_SaveEdit.hide();
                $(document).ready(function()
                {{
                    {0}_SaveEdit.find('script').remove();
                    $('#{0}___Choice1___flattened_ItemsContainer').find('script').remove();
                    setTimeout(function(){{{0}_SaveEdit.detach(); {0}_SaveEdit.show();}});
                }});
            </script>
        ";
        private static string completeTreeScriptEdit =
        @"
            <script language='javascript' type='text/javascript'>
                var {0}_SaveDisplay = null;
                var {0}_SaveEdit = null;
                var {0}_ButtonMode = 2; 
                {0}_SaveDisplay = $('#{0}___Choice1___flattened_ItemsContainer');
                {0}_SaveDisplay.hide();
                $(document).ready(function()
                {{ 
                    {0}_SaveDisplay.find('script').remove();
                    $('#{0}___Choice2___flattened_ItemsContainer').find('script').remove();
                    {0}_SaveDisplay.detach();
                    {0}_SaveDisplay.show();
                }});
            </script>
        ";
        private static string templateSymbol = "QS_23459_86{0}45_ZUT";
        private static string startScriptFormat =
        @"
            <script language='javascript' type='text/javascript'>
                var {0}_RootNamePostfix='{3}';
                $(document).ready(function()
                {{

                    var jQueryRoot = $('#{0}_ItemsContainer');
                    jQueryRoot.sortable({{ handle: '.{0}_handle',   {1} {2} 
                                update: function(event, ui) {{MvcControlsToolkit_TreeView_UpdatePermutations(ui.item, '{0}_ItemsContainer'); }},
                                start:  function(event, ui) {{MvcControlsToolkit_TreeView_StartDrag(ui.item, jQueryRoot);}},
                                stop:   function(event, ui) {{MvcControlsToolkit_TreeView_StopDrag(ui.item, jQueryRoot);}}
                                }});
                    
                }});
            </script>
        ";
        private static string startScriptNoMoveFormat =
        @"
            <script language='javascript' type='text/javascript'>
                var {0}_RootNamePostfix='{1}';
               
            </script>
        ";

        private static string levelSelection =
       @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function()
                {{
                    $('.level-select_{0}').change(
                        function(event){{
                            MvcControlsToolkit_TreeView_SelectLevel(event.target, '.level-select_{0}');
                        }}
                    );
                }});
            </script>
        ";
        private static string startTemplateScriptFormat =
       @"
            <script language='javascript' type='text/javascript'>
                 var {0}_CanSort = {1};
                 var {0}_TemplateSymbol = /{2}/g;
                 var {0}_TemplateSript = null;
                 var {0}_TemplateHtml = null;
                $(document).ready(function()
                {{
                   
                    MvcControlsToolkit_TreeView_PrepareTemplates('{0}', [{3}]); 
                }});
            
            </script>
        ";
        private static string toggleEditButtonScript = "MvcControlsToolkit_TreeView_ToggleEdit(\"{0}\", \"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\");";
        private static string initializeToggleEditButtonScript =
        @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function()
                {{                
                    MvcControlsToolkit_TreeView_AdjustToggleButton('{0}', '{1}','{2}','{3}','{4}','{5}','{6}');
                }});
            </script>
        ";

        private static string addButtonScript = "MvcControlsToolkit_TreeView_AddNew(\"{0}\", {1});";
        private static string removeButtonScript = "MvcControlsToolkit_TreeView_Delete(\"{0}\");";


        public static MvcHtmlString TreeViewAddButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            int templateToUse,
            string textOrUrl,
             ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null,
            string name = null)
        {
            if (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient) return MvcHtmlString.Create(string.Empty);
            string id = htmlHelper.PrefixedId("ItemsContainer");
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl,
                                  string.Format(addButtonScript, id, templateToUse), name == null ? id + "_AddButton" + templateToUse.ToString() : name, manipulationButtonStyle, htmlAttributes);
        }
        public static MvcHtmlString TreeViewDeleteButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            string textOrUrl,
             ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null,
            string name = null)
        {
            
            
            string id = htmlHelper.PrefixedId("Container");
            return htmlHelper.ManipulationButton(ManipulationButtonType.Custom, textOrUrl,
                                  string.Format(removeButtonScript, id), name == null ? id + "_RemoveButton" : name, manipulationButtonStyle, htmlAttributes);
        }

        public static MvcHtmlString TreeViewToggleEditButtonFor<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<T>>> expression,
            string textOrUrlEdit,
            string cssClassEdit,
            string textOrUrlUndoEdit,
            string cssClassUndoEdit,
            string textOrUrlRedoEdit,
            string cssClassRedoEdit,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));

            if (textOrUrlEdit ==null) throw (new ArgumentNullException("textOrUrlEdit"));
            if (cssClassEdit == null) throw (new ArgumentNullException("cssClassEdit"));

            if (textOrUrlUndoEdit == null) throw (new ArgumentNullException("textOrUrlUndoEdit"));
            if (cssClassUndoEdit == null) throw (new ArgumentNullException("cssClassUndoEdit"));

            if (textOrUrlRedoEdit == null) throw (new ArgumentNullException("textOrUrlRedoEdit"));
            if (cssClassRedoEdit == null) throw (new ArgumentNullException("cssClassRedoEdit"));

            if (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient) return MvcHtmlString.Create(string.Empty);
            string id = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression)));
            return MvcHtmlString.Create(
                string.Format(initializeToggleEditButtonScript, id, textOrUrlEdit, cssClassEdit, textOrUrlUndoEdit, cssClassUndoEdit, textOrUrlRedoEdit, cssClassRedoEdit) +
                htmlHelper.ManipulationButton(ManipulationButtonType.Custom, string.Empty,
                                  string.Format(toggleEditButtonScript, id, textOrUrlEdit, cssClassEdit, textOrUrlUndoEdit, cssClassUndoEdit, textOrUrlRedoEdit, cssClassRedoEdit), 
                                  id + "_ToggleEditButton", manipulationButtonStyle, null).ToString());
        }
        private static bool isClosed(TreeViewItemStatus status, object item, HtmlHelper htmlHelper)
        {
            IDictionary dict = htmlHelper.ViewContext.RequestContext.HttpContext.Items;
            switch (status)
            {
                case TreeViewItemStatus.Hide: return true;
                case TreeViewItemStatus.Show: return false;
                case TreeViewItemStatus.InitializeHide:
                    if (dict.Contains(item)) return (bool)dict[item];
                    else return true;
                default:
                    if (dict.Contains(item)) return (bool)dict[item];
                    else return false;
            }
        }
        private static void addTemplateId(StringBuilder sb, string templateId)
        {
            if (sb.Length > 0) sb.Append(", ");
            if (templateId == null) templateId = string.Empty;
            sb.Append("'");
            sb.Append(templateId);
            sb.Append("'");
        }
        private static void TreeViewTop<VM, T>(
            StringBuilder sb,
            bool editMode,
            HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<T>> renderInfo,
            Func<int, string> collectionName,
            ExternalContainerType itemContainer,
            string rootClass,
            object[] itemTemplates,
            Func<object, int, int> itemTemplateSelector,
            Func<int, string> itemClassSelector,
            Func<object, int, TreeViewItemStatus> itemStatus,
            float opacity,
            bool canMove,
            bool canAdd,
            TreeViewOptions treeViewOptions)
        {
            if (rootClass == null) rootClass = defaultRootClass;
            

            RenderInfo <TreeViewDisplay<T>> branchesRenderInfo = htmlHelper.InvokeTransform(renderInfo, new TreeViewDisplay<T>());
            if(editMode) sb.Append(branchesRenderInfo.PartialRendering);
            
            renderInfo.Prefix = BasicHtmlHelper.AddField(branchesRenderInfo.Prefix, "flattened");
            renderInfo.PartialPrefix = BasicHtmlHelper.AddField(branchesRenderInfo.PartialPrefix, "flattened");
            
            StringBuilder sbInit = new StringBuilder();
            sbInit.Append(treeViewOptions.Render(renderInfo.Prefix));
            string basic_id=BasicHtmlHelper.IdFromName(renderInfo.Prefix);
            int res = TreeViewRec<VM, T>(
            sb,
            sbInit,
            editMode,
            htmlHelper,
            renderInfo,
            collectionName,
            itemContainer,
            rootClass,
            itemTemplates,
            itemTemplateSelector,
            itemClassSelector,
            itemStatus,
            opacity,
            canMove,
            0,
            0,
            basic_id,
            basic_id);
            if (canAdd)
            {
                StringBuilder templatesId = new StringBuilder();
                string myTemplateSymbol = BasicHtmlHelper.GetUniqueSymbol(htmlHelper, templateSymbol);
                sb.AppendFormat("<div id='{0}_Templates'>", basic_id);
                int templateIndex = -1;
                IDictionary<string, object> htmlAttributesContainer = new Dictionary<string, object>();
                foreach (object template in itemTemplates)
                {
                    templateIndex++;
                    Type type = TemplateInvoker<string>.ExtractModelType(template);
                    string initCollection = collectionName(templateIndex);
                    
                    ITreeViewNodeContainer wrapper =
                        typeof(TreeViewUpdater<string>).GetGenericTypeDefinition()
                        .MakeGenericType(new Type[] { type })
                        .GetConstructor(new Type[0])
                        .Invoke(new object[0]) as ITreeViewNodeContainer;
                        

                    bool closed = false;
                    (wrapper as IUpdateModel).ImportFromModel(null, null, null, new object[] { closed });

                    string prefix = renderInfo.Prefix;
                    string partialPrefix = renderInfo.PartialPrefix;
                    string updater = BasicHtmlHelper.RenderUpdateInfoI(
                        htmlHelper, wrapper as IUpdateModel,
                        ref partialPrefix, new string[0], myTemplateSymbol+templateIndex.ToString());
                    
                    prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);

                    

                    wrapper.FatherOriginalId = string.Empty;
                    wrapper.OriginalId = prefix;
                    wrapper.PositionAsSon = 0;
                    wrapper.SonNumber = 0;
                    wrapper.SonCollectionName = initCollection;
                    wrapper.Closed = false;
                    string itemOpenTag = null;
                    string itemCloseTag = null;

                    string innerItemOpenTag = null;
                    string innerItemCloseTag = null;
                    
                    string templateId = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "Container"));
                    string templateUniqueId = templateId + templateIndex.ToString();
                    addTemplateId(templatesId, templateUniqueId);
                    htmlAttributesContainer["id"] = templateId;
                    htmlAttributesContainer["class"] = closed ? "closed" : "open";
                    BasicHtmlHelper.GetContainerTags(ExternalContainerType.li, htmlAttributesContainer, out itemOpenTag, out itemCloseTag);

                    htmlAttributesContainer["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item_SubContainer"));
                    htmlAttributesContainer["class"] = string.Empty;
                    BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesContainer, out innerItemOpenTag, out innerItemCloseTag);

                    bool hasCollection = true;

                    PropertyInfo property = null;
                    if (initCollection == null) hasCollection = false;
                    else
                    {
                        property = type.GetProperty(initCollection);
                        hasCollection = typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
                    }
                    sb.Append(string.Format("<span id='{0}' style='display:none' class='MVCCT_EncodedTemplate'>", templateUniqueId));
                    StringBuilder itemSb = new StringBuilder();
                    itemSb.Append(itemOpenTag);

                    if (canMove && hasCollection) itemSb.Append(string.Format("<input  type='checkbox' class ='level-select_{0} level-select' />", basic_id));
                    itemSb.Append(innerItemOpenTag);
                    
                    itemSb.Append(
                        (typeof(TemplateInvoker<string>)
                        .GetGenericTypeDefinition()
                        .MakeGenericType(new Type[]{type})
                        .GetConstructor(new Type[]{typeof(object)})
                        .Invoke(new object[]{template}) as ITemplateInvoker)
                        .Invoke(htmlHelper, null, BasicHtmlHelper.AddField(prefix, "$.Item")));
                    itemSb.Append(innerItemCloseTag);

                    itemSb.Append(string.Format("<span class= 'MvcCT_init_info_{0}'>", basic_id));
                    itemSb.Append(updater);
                    RenderWrapper<VM>(htmlHelper, partialPrefix, wrapper, itemSb); 
                    itemSb.Append("</span>");

                    if (hasCollection)
                    {
                        string base_id = BasicHtmlHelper.IdFromName(prefix);
                        string currItemClass = itemClassSelector == null ?
                            null :
                            itemClassSelector(templateIndex);
                        if (currItemClass != null)
                            currItemClass = currItemClass + "_" + basic_id;
                        else
                            currItemClass = string.Empty;
                        string externalOpenTag = null;
                        string externalCloseTag = null;
                        htmlAttributesContainer["id"] = base_id + "_ItemsContainer";

                        htmlAttributesContainer["class"] = currItemClass + " mvcct-items-container";
                        
                        BasicHtmlHelper.GetContainerTags(ExternalContainerType.ul, htmlAttributesContainer, out externalOpenTag, out externalCloseTag);
                        
                        itemSb.Append(externalOpenTag);
                        itemSb.Append(externalCloseTag);
                        if (canMove)
                        {
                            string javasctiptOpacity = string.Empty;
                            if (opacity > 1f) opacity = 1f;
                            else if (opacity < 0.01f) opacity = 0.01f;

                            if (opacity < 1f)
                            {
                                javasctiptOpacity = string.Format(" opacity: {0}, ", opacity.ToString(CultureInfo.InvariantCulture));
                            }
                            string javascriptRootClass = string.Empty;
                            if (currItemClass != null) javascriptRootClass = string.Format(" connectWith: '.{0}', ", currItemClass);
                            
                            itemSb.Append(string.Format(startScriptFormat,
                                base_id,
                                javasctiptOpacity,
                                javascriptRootClass,
                                basic_id));
                            
                        }
                    }
                    itemSb.Append(itemCloseTag);
                    sb.Append(htmlHelper.Encode(itemSb.ToString()));
                    sb.Append("</span>");
                }
                sb.Append("</div>");
                sb.AppendFormat(
                    startTemplateScriptFormat,
                    basic_id, canMove ? "true" : "false",
                    myTemplateSymbol,
                    templatesId.ToString());

            }
            if(editMode)
                sbInit.Append(htmlHelper.Hidden(BasicHtmlHelper.AddField(renderInfo.PartialPrefix, "$.ItemsCount"),
                    res).ToString());
            if (canMove) sbInit.Append(string.Format(levelSelection, basic_id));
            sb.Append(sbInit.ToString());
        }
        private static void RenderWrapper<VM>(HtmlHelper<VM> htmlHelper, string partialPrefix, ITreeViewNodeContainer wrapper, StringBuilder sbInit)
        {
            
            sbInit.Append(htmlHelper.GenericInput(InputType.Hidden, BasicHtmlHelper.AddField( partialPrefix, "$.Closed"),
                wrapper.Closed).ToString());
            sbInit.Append(htmlHelper.GenericInput(InputType.Hidden, BasicHtmlHelper.AddField(partialPrefix, "$.FatherOriginalId"),
                wrapper.FatherOriginalId).ToString());
            sbInit.Append(htmlHelper.GenericInput(InputType.Hidden, BasicHtmlHelper.AddField(partialPrefix, "$.OriginalId"),
                wrapper.OriginalId).ToString());
            sbInit.Append(htmlHelper.GenericInput(InputType.Hidden, BasicHtmlHelper.AddField(partialPrefix, "$.PositionAsSon"),
                wrapper.PositionAsSon).ToString());
            sbInit.Append(htmlHelper.GenericInput(InputType.Hidden, BasicHtmlHelper.AddField(partialPrefix, "$.SonCollectionName"),
                wrapper.SonCollectionName).ToString());
            sbInit.Append(htmlHelper.GenericInput(InputType.Hidden, BasicHtmlHelper.AddField(partialPrefix, "$.SonNumber"),
                wrapper.SonNumber).ToString());
            
        }
        private static int TreeViewRec<VM, T> (
            StringBuilder sb,
            StringBuilder sbInit,
            bool editMode,
            HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<T>> renderInfo,
            Func<int, string> collectionName,
            ExternalContainerType itemContainer,
            string rootClass,
            object[] itemTemplates,
            Func<object, int, int> itemTemplateSelector,
            Func<int, string> itemClassSelector,
            Func<object, int, TreeViewItemStatus>  itemStatus,
            float opacity,
            bool canMove,
            int level,
            int totalCount,
            string fatherName,
            string root_id)
        {
            string basicId = BasicHtmlHelper.IdFromName(fatherName);
            

            
            sbInit.Append(renderInfo.PartialRendering);
            
            IDictionary<string, object> htmlAttributesContainer = new Dictionary<string, object>();
            
            string externalOpenTag = null;
            string externalCloseTag = null;
            string handleClass = basicId + "_handle";
            
            htmlAttributesContainer["id"] = basicId + "_ItemsContainer";
            if (level == 0)
            {

                htmlAttributesContainer["class"] = rootClass + "  mvcct-items-container";
                rootClass = null;
            }
            else
            {
                if (rootClass != null) htmlAttributesContainer["class"] = rootClass+"_  mvcct-items-container";
            }
            BasicHtmlHelper.GetContainerTags(ExternalContainerType.ul, htmlAttributesContainer, out externalOpenTag, out externalCloseTag);
            sb.Append(externalOpenTag);

            IEnumerable list = renderInfo.Model as IEnumerable;
            if (list == null) list = new List<T>();
            
            string javasctiptOpacity = string.Empty;
            int sonIndex =-1;;
            foreach (object o in list)
            {
                if (o == null) continue;
                totalCount++;
                sonIndex++;
                int templateIndex = itemTemplateSelector(o, level);
                object initialTemplate = itemTemplates[templateIndex];
                string initCollection = collectionName(templateIndex);
                
                
                TreeViewItemStatus status = itemStatus(o, level);
                if (initCollection == null) status = TreeViewItemStatus.Hide;
                 ITreeViewNodeContainer wrapper=null;
                 IUpdateModel uWrapper=null;
                 bool closed = isClosed(status, o, htmlHelper);
                 Type type = TemplateInvoker<string>.ExtractModelType(initialTemplate);
                 if (editMode)
                 {
                     wrapper =
                         typeof(TreeViewUpdater<string>).GetGenericTypeDefinition()
                         .MakeGenericType(new Type[] { type })
                         .GetConstructor(new Type[0])
                         .Invoke(new object[0]) as ITreeViewNodeContainer;
                     uWrapper = wrapper as IUpdateModel;
                     uWrapper.ImportFromModel(o, null, null, new object[] { closed });

                 }
                 else
                 {
                     wrapper = new TreeViewUpdater<T>(false);
                     uWrapper = wrapper as IUpdateModel;
                 }
                
                

                string prefix = renderInfo.Prefix;
                string partialPrefix = renderInfo.PartialPrefix;
                if (editMode)
                {
                    sbInit.Append(
                        BasicHtmlHelper.RenderUpdateInfoI(htmlHelper, uWrapper, ref partialPrefix, new string[0]));
                }
                else
                {
                    BasicHtmlHelper.RenderUpdateInfoI(htmlHelper, uWrapper, ref partialPrefix, new string[0], noOutput: true);
                }
                prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                if (level == 0) wrapper.FatherOriginalId =null;
                else wrapper.FatherOriginalId = fatherName;
                wrapper.OriginalId = prefix;
                wrapper.PositionAsSon = sonIndex;
                wrapper.SonNumber = 0;
                wrapper.SonCollectionName = null;
                string itemOpenTag = null;
                string itemCloseTag = null;

                string innerItemOpenTag = null;
                string innerItemCloseTag = null;

                htmlAttributesContainer["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "Container"));
                htmlAttributesContainer["class"] = closed ? "closed" : "open";
                BasicHtmlHelper.GetContainerTags(ExternalContainerType.li, htmlAttributesContainer, out itemOpenTag, out itemCloseTag);

                htmlAttributesContainer["id"] = BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item_SubContainer"));
                htmlAttributesContainer["class"] = handleClass;
                BasicHtmlHelper.GetContainerTags(itemContainer, htmlAttributesContainer, out innerItemOpenTag, out innerItemCloseTag);
                

                bool hasCollection = true;
                string name = initCollection;
                PropertyInfo property = null;
                if (name == null) hasCollection = false;
                else
                {
                    property = o.GetType().GetProperty(name);
                    hasCollection = typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
                }
                sb.Append(itemOpenTag);
                
                if (canMove && hasCollection)
                    sb.Append(string.Format("<input  type='checkbox' class ='level-select_{0} level-select' />", root_id));
                sb.Append(innerItemOpenTag);
                sb.Append(
                        (typeof(TemplateInvoker<string>)
                        .GetGenericTypeDefinition()
                        .MakeGenericType(new Type[]{type})
                        .GetConstructor(new Type[]{typeof(object)})
                        .Invoke(new object[]{initialTemplate}) as ITemplateInvoker)
                        .Invoke(htmlHelper,o, BasicHtmlHelper.AddField(prefix, "$.Item")));
                sb.Append(innerItemCloseTag);
                
                if (hasCollection)
                {
                    string currItemClass = itemClassSelector == null ?
                            null :
                            itemClassSelector(templateIndex);
                    if (currItemClass != null)
                        currItemClass = currItemClass.Trim() + "_" + BasicHtmlHelper.IdFromName(renderInfo.Prefix);
                    ICollection innerItem = property.GetValue(o, new object[0]) as ICollection;
                    Type listType = property.PropertyType.GetGenericArguments()[0];
                    Type allListType = typeof(IEnumerable<string>).GetGenericTypeDefinition().MakeGenericType(listType);
                    if (innerItem == null)
                    {
                        innerItem=allListType.GetConstructor(new Type[0]).Invoke(new object[0]) as ICollection;
                    }
                    wrapper.SonNumber = innerItem.Count;
                    wrapper.SonCollectionName = initCollection;
                    if(editMode) RenderWrapper<VM>(htmlHelper, partialPrefix, wrapper, sbInit);
                    
                    totalCount = (int)typeof(TreeViewHelpers).GetMethod("TreeViewRec", BindingFlags.Static|BindingFlags.NonPublic).
                        MakeGenericMethod(new Type[] { typeof(VM),  listType})
                        .Invoke(null, new object[]
                        {sb,
                        sbInit,
                        editMode,
                        htmlHelper,
                        typeof(RenderInfo<string>).GetGenericTypeDefinition().MakeGenericType(allListType)
                            .GetConstructor(new Type[]{typeof(string), typeof(string), typeof(string), allListType})
                            .Invoke
                        (new object[]{
                            renderInfo.Prefix,
                            renderInfo.PartialPrefix, 
                            string.Empty,
                            innerItem
                        }),
                        collectionName,
                        itemContainer,
                        currItemClass,
                        itemTemplates,
                        itemTemplateSelector,
                        itemClassSelector,
                        itemStatus,
                        opacity,
                        canMove,
                        level + 1,
                        totalCount,
                        prefix,
                        root_id});
                }
                else
                {
                    if (editMode) RenderWrapper<VM>(htmlHelper, partialPrefix, wrapper, sbInit);
                }
                sb.Append(itemCloseTag);
            }


            if (canMove)
            {
                if (opacity > 1f) opacity = 1f;
                else if (opacity < 0.01f) opacity = 0.01f;

                if (opacity < 1f)
                {
                    javasctiptOpacity = string.Format(" opacity: {0}, ", opacity.ToString(CultureInfo.InvariantCulture));
                }
                string javascriptRootClass = string.Empty;
                if (rootClass != null) javascriptRootClass = string.Format(" connectWith: '.{0}', ", rootClass);
                if (level > 0)
                {
                    sbInit.Append(string.Format(startScriptFormat,
                        basicId,
                        javasctiptOpacity,
                        javascriptRootClass,
                        root_id));
                }
            }
            else
            {
                if (level > 0)
                {
                    sbInit.Append(string.Format(startScriptNoMoveFormat,
                        basicId,
                        root_id));
                }
            }
            sb.Append(externalCloseTag);

            
            return totalCount;
        }

        public static MvcHtmlString TreeViewFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<TItem>> renderInfo,
            Func<int, string> collectionName,
            ExternalContainerType itemContainer=ExternalContainerType.span,
            string rootClassDisplay = null,
            object[] displayTemplates = null,
            Func<object, int, int> itemTemplateSelectorDisplay=null, 
            string rootClassEdit = null,
            object[] editTemplates = null,
            Func<object, int, int> itemTemplateSelectorEdit=null,
            TreeViewMode mode = TreeViewMode.InitializeDisplay,
            Func<int, string> itemClassSelector=null,
            Func<object, int, TreeViewItemStatus>  itemStatus = null,
            TreeViewOptions treeViewOptions=null)
        {
            if (renderInfo == null) throw(new ArgumentNullException("renderInfo"));
            if (collectionName == null) throw(new ArgumentNullException("collectionName"));
            if ((displayTemplates == null || itemTemplateSelectorDisplay == null || displayTemplates.Length == 0) &&
                (editTemplates == null || itemTemplateSelectorEdit==null ||editTemplates.Length == 0))
                throw (new ArgumentNullException("displayTemplates/itemTemplateSelectorDisplay or editTemplates/itemTemplateSelectorEdit"));
            if (itemStatus==null) itemStatus=(mbox, i) => TreeViewItemStatus.initializeShow;
            if (treeViewOptions == null) treeViewOptions = new TreeViewOptions();
            
            if (MvcEnvironment.Validation(htmlHelper) == ValidationType.StandardClient)
            {
                treeViewOptions.CanAdd = false;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(renderInfo.PartialRendering);

            RenderInfo<IEnumerable<TItem>> renderInfoO = new RenderInfo<IEnumerable<TItem>>();
            renderInfoO.Model = renderInfo.Model;
            renderInfoO.PartialPrefix = renderInfo.PartialPrefix;
            renderInfoO.Prefix = renderInfo.Prefix;
            renderInfoO.PartialRendering = renderInfo.PartialRendering;

            

            
            if (editTemplates == null || editTemplates.Length == 0)
            {
                TreeViewTop<VM, TItem>(
                    sb, false, htmlHelper, renderInfoO, collectionName, itemContainer,
                    rootClassDisplay, displayTemplates, itemTemplateSelectorDisplay,
                    itemClassSelector, itemStatus, treeViewOptions.Opacity, false, false, treeViewOptions);
                
            }
            else if (displayTemplates == null || displayTemplates.Length == 0)
            {
                TreeViewTop<VM, TItem>(
                    sb, true, htmlHelper, renderInfoO, collectionName, itemContainer,
                    rootClassEdit, editTemplates, itemTemplateSelectorEdit,
                    itemClassSelector, itemStatus, treeViewOptions.Opacity, treeViewOptions.CanMove, treeViewOptions.CanAdd, treeViewOptions);
                
            }
            else
            {
                bool isEdit = mode == TreeViewMode.Edit || mode == TreeViewMode.InitializeEdit;
                if (mode == TreeViewMode.InitializeEdit || mode == TreeViewMode.InitializeDisplay)
                {
                    IDictionary vars = htmlHelper.ViewContext.RequestContext.HttpContext.Items;
                    if (vars.Contains(renderInfo.Prefix))
                    {
                        isEdit = (bool)(vars[renderInfo.Prefix]);
                    }
                }
                string toggleScript=null;
                if (isEdit)
                {
                    toggleScript=string.Format(completeTreeScriptEdit,
                        BasicHtmlHelper.IdFromName(renderInfo.Prefix));
                }
                else
                {
                    toggleScript=string.Format(completeTreeScriptDisplay,
                        BasicHtmlHelper.IdFromName(renderInfo.Prefix));
                }

                RenderInfo<TwoWayChoice<IEnumerable<TItem>>> toRender = htmlHelper.InvokeTransform(renderInfo, new TwoWayChoice<IEnumerable<TItem>>());
                toRender.Model.IsChoice2 = isEdit;
                sb.Append(toRender.PartialRendering);
                sb.Append(htmlHelper.Hidden(BasicHtmlHelper.AddField(toRender.PartialPrefix, "IsChoice2"), toRender.Model.IsChoice2));
                renderInfoO.Model = toRender.Model.Choice1;
                renderInfoO.PartialPrefix = BasicHtmlHelper.AddField(toRender.PartialPrefix, "Choice1");
                renderInfoO.Prefix = BasicHtmlHelper.AddField(toRender.Prefix, "Choice1");
                renderInfoO.PartialRendering = string.Empty;

                TreeViewTop<VM, TItem>(
                    sb, false, htmlHelper, renderInfoO, collectionName, itemContainer,
                    rootClassDisplay, displayTemplates, itemTemplateSelectorDisplay,
                    itemClassSelector, itemStatus, treeViewOptions.Opacity, false, false, treeViewOptions);
                
                
                renderInfoO.Model = toRender.Model.Choice2;
                renderInfoO.PartialPrefix = BasicHtmlHelper.AddField(toRender.PartialPrefix, "Choice2");
                renderInfoO.Prefix = BasicHtmlHelper.AddField(toRender.Prefix, "Choice2");
                

                TreeViewTop<VM, TItem>(
                    sb, true, htmlHelper, renderInfoO, collectionName, itemContainer,
                    rootClassEdit, editTemplates, itemTemplateSelectorEdit,
                    itemClassSelector, itemStatus, treeViewOptions.Opacity, treeViewOptions.CanMove, treeViewOptions.CanAdd, treeViewOptions);

                sb.Append(toggleScript);
                
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString TreeViewFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM,IEnumerable<TItem>>> expression,
            Func<int, string>  collection,
            ExternalContainerType itemContainer=ExternalContainerType.span,
            string rootClassDisplay = null,
            object[] displayTemplates = null,
            Func<object, int, int> itemTemplateSelectorDisplay= null, 
            string rootClassEdit = null,
            object[] editTemplates = null,
            Func<object, int, int> itemTemplateSelectorEdit=null,
            TreeViewMode mode = TreeViewMode.InitializeDisplay,
            Func<int, string> itemClassSelector = null,
            Func<object, int, TreeViewItemStatus> itemStatus = null,
            TreeViewOptions treeViewOptions = null)
        {
            if (expression == null) throw(new ArgumentNullException("expression"));
            if (collection == null) throw(new ArgumentNullException("collection"));
            if ((displayTemplates == null || displayTemplates.Length == 0) &&
                (editTemplates == null || editTemplates.Length == 0))
                throw (new ArgumentNullException("displayTemplates editTemplates"));
            
            return TreeViewFor
                (htmlHelper,
                htmlHelper.ExtractFromModel(expression),
                collection,
                itemContainer,
                rootClassDisplay,
                displayTemplates,
                itemTemplateSelectorDisplay,
                rootClassEdit,
                editTemplates,
                itemTemplateSelectorEdit,
                mode,
                itemClassSelector,
                itemStatus,
                treeViewOptions);
        }
            

    }
}

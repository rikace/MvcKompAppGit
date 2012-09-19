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
using System.Web.Mvc.Ajax;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controller;
using MVCControlsToolkit.Controls.DataGrid;
using System.Reflection;



namespace MVCControlsToolkit.Controls
{
 
    public static class DataFormHelpers
    {
        private static string prefixTranslationScript =
            @"
            <script language='javascript' type='text/javascript'>
              var {0}_DetailPrefix = '{1}';
            </script>   
            ";
        private static string fieldsToUpdateScript =
        @"
            <script language='javascript' type='text/javascript'>
              if ({0}_FieldsToUpdate == null) {0}_FieldsToUpdate = '{1}';
              var {0}_DetailBusy = false;
              var {0}_CurrentRow = null;
              var {0}_TypeDetail = null;
              var {0}_ChangedFieldCss = {2};
              var {0}_DeletedRecordCss = {3};
            </script>   
            ";//TypeDetail = Insert, Edit, FirstEdit, Display
        private static string onBeginScript =
            @"
            <script language='javascript' type='text/javascript'>
              function {0}(){{
                return OnBeginDetailForm('{1}', '{2}', {3}, '{4}'); 
              }}
            </script>
            ";
        private static string onFailureScript =
           @"
            <script language='javascript' type='text/javascript'>
              function {0}(){{
                OnFailureDetailForm('{1}', {2}, {3}); 
              }}
            </script>
            ";
        private static string onSuccessScript =
           @"
            <script language='javascript' type='text/javascript'>
              function {0}(ajaxContext){{
                {4}
                OnSuccessDetailForm('{1}', {2}, {3}, ajaxContext, '{5}', '{6}', {7}, '{8}'); 
                
              }}
            </script>
            ";
        
        private static void getPropertiesToUpdate(StringBuilder sb, string prefix, PropertyInfo[] propertiesToUpdate, Stack<Type> recursionControl)
        {
            for (int i = 0; i < propertiesToUpdate.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(',');
                }
                if (typeof(IConvertible).IsAssignableFrom(propertiesToUpdate[i].PropertyType))
                {
                    sb.Append(BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, propertiesToUpdate[i].Name)));
                }
                else if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(propertiesToUpdate[i].PropertyType))
                {
                    bool recursion = false;
                    foreach(Type type in recursionControl)
                    {
                        if (type == propertiesToUpdate[i].PropertyType) recursion=true;
                    }
                    if (recursion) continue;
                    recursionControl.Push(propertiesToUpdate[i].PropertyType);
                    getPropertiesToUpdate(
                        sb,
                        BasicHtmlHelper.AddField(prefix, propertiesToUpdate[i].Name),
                        BasicHtmlHelper.GetPropertiesForInput(propertiesToUpdate[i].PropertyType), recursionControl);
                    recursionControl.Pop();
                }
            }
        }
        private static string syncScript =
        @"
        <script language='javascript' type='text/javascript'>
            {0}
            {1}
            {2}
        </script>
            {3}
        ";
        public static MvcHtmlString DetailFormSyncInfos<TItem, T>
            (this HtmlHelper<TItem> htmlHelper, 
            Expression<Func<TItem, T>> expression,
            string formattedValue,
            bool htmlEncode = true,
            string urlValue=null)
        {
            if (expression == null) throw (new ArgumentException("expression"));
            string trueValue = string.Empty;
            string id = BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(
                    htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                    ExpressionHelper.GetExpressionText(expression)));

            if (!typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                T model = default(T);
                try{
                        model = expression.Compile().Invoke(htmlHelper.ViewData.Model);
                }
                catch{
                }
                trueValue = string.Format(
                    "var {0}_True = '{1}'; ", 
                    id, 
                    htmlHelper.Encode(BasicHtmlHelper.ClientEncode(model)));
            }
            string fUrlValue = string.Empty;
            if (urlValue != null)
            {
                fUrlValue = string.Format(
                    "var {0}_Url = '{1}'; ",
                    id,
                    htmlHelper.Encode(urlValue));
            }
            string fFormattevValue = string.Empty;
            string html = string.Empty;
            if (formattedValue != null)
            {
                if (htmlEncode)
                {
                    fFormattevValue = string.Format(
                        "var {0}_Format = '{1}'; ",
                        id,
                        htmlHelper.Encode(formattedValue));
                }
                else
                {
                    html = string.Format(
                        @"<span id='{0}_Format' style='display:none'>
                        {1}
                        </span>
                        ",
                        id,
                        htmlHelper.Encode(formattedValue));
                    fFormattevValue = string.Format(
                        @"var {0}_Format = '<>'; ",
                        id);
                }
            }
            return MvcHtmlString.Create(
                string.Format(syncScript, trueValue, fUrlValue, fFormattevValue, html));

        }
        public static void DetailFormFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            AjaxHelper<VM> ajax,
            RenderInfo<IEnumerable<Tracker<TItem>>> renderInfo,
            ExternalContainerType externalContainerType,
            string postActionName,
            string postControllerName,
            string detailPrefix=null,
            string changedFieldCss=null,
            string deletedRecordCss = null,
            IDictionary<string, object> externalContainerHtmlattributes=null,
            string clientOnSuccessEdit = null,
            string clientOnFailureEdit = null,
            string clientOnSuccessDisplay = null,
            string clientOnFailureDisplay = null,
            string loadingElementId = null,
            Dialog detailDialog = null
            )
            where TItem:class, new()
        {
            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));
            if (ajax == null) throw (new ArgumentNullException("ajax"));
            if (postActionName == null) throw (new ArgumentNullException("postActionName"));
            if (postControllerName == null) throw (new ArgumentNullException("postControllerName"));
            if (externalContainerHtmlattributes == null) externalContainerHtmlattributes = new Dictionary<string, object>();
            string baseName=BasicHtmlHelper.IdFromName(renderInfo.Prefix);
            string formName = baseName + "_AjaxForm";
            string containerName= baseName+"_Container";
            string onSuccessName = baseName+"_OnSuccess";
            string onFailureName = baseName + "_OnFailure";
            string onBeginName = baseName + "_OnBegin";
            string beginTag;
            string closeTag;
            externalContainerHtmlattributes["id"]=containerName;
            BasicHtmlHelper.GetContainerTags(externalContainerType, externalContainerHtmlattributes, out beginTag, out closeTag);

            string javascriptChangedFieldCss = changedFieldCss == null ? "null" : "'" + changedFieldCss + "'";
            string javascriptDeletedRecordCss = deletedRecordCss == null ? "null" : "'" + deletedRecordCss + "'";

            string javascriptClientOnSuccessEdit = clientOnSuccessEdit == null ? "null" : "'" + clientOnSuccessEdit + "'";
            string javascriptClientOnSuccessDisplay = clientOnSuccessDisplay == null ? "null" : "'" + clientOnSuccessDisplay + "'";

            string javascriptClientOnFailureEdit = clientOnFailureEdit == null ? "null" : "'" + clientOnFailureEdit + "'";
            string javascriptClientOnFailureDisplay = clientOnFailureDisplay == null ? "null" : "'" + clientOnFailureDisplay + "'";
            if (detailPrefix == null) detailPrefix = string.Empty;

            string validationType = null;
            string unobtrusiveAjaxOn = "false";
            if (MvcEnvironment.UnobtrusiveAjaxOn(htmlHelper)) unobtrusiveAjaxOn = "true";
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }

            htmlHelper.ViewContext.Writer.Write(
                string.Format(prefixTranslationScript,
                    baseName,
                    BasicHtmlHelper.IdFromName(detailPrefix)));
            
            PropertyInfo[] propertiesToUpdate =
                BasicHtmlHelper.GetPropertiesForInput(typeof(TItem));
            
            StringBuilder sb = new StringBuilder();
            
            
            Stack<Type> recursionControl = new Stack<Type>();

            recursionControl.Push(typeof(TItem));
            getPropertiesToUpdate(
                sb,
                string.Empty,
                propertiesToUpdate, 
                recursionControl);
            recursionControl.Pop();

            object formHtmlAttributes = null;
            if (formName!=null) {
                formHtmlAttributes = new { id = formName };
                
                }
            htmlHelper.ViewContext.Writer.Write(
                string.Format(fieldsToUpdateScript, baseName, sb.ToString(), javascriptChangedFieldCss, javascriptDeletedRecordCss));
            htmlHelper.ViewContext.Writer.Write(
                    string.Format(onBeginScript, onBeginName, baseName, "Edit", "null", loadingElementId, validationType));
            string dialogOpen;
            if (detailDialog == null)
            {
                dialogOpen = string.Empty;
            }
            else
            {
                dialogOpen = detailDialog.GetShow('#' + formName);
            }

            htmlHelper.ViewContext.Writer.Write(
                    string.Format(onSuccessScript, onSuccessName, baseName, javascriptClientOnSuccessDisplay, javascriptClientOnSuccessEdit, dialogOpen, formName, validationType, unobtrusiveAjaxOn, containerName));
            htmlHelper.ViewContext.Writer.Write(
                    string.Format(onFailureScript, onFailureName, baseName, javascriptClientOnFailureDisplay, javascriptClientOnFailureEdit));
             
            if (detailDialog != null)
            {
                htmlHelper.ViewContext.Writer.Write(detailDialog.GetCreation('#' + formName));
            }
            using (var form = ajax.BeginForm(
                postActionName,
                postControllerName,
                new AjaxOptions()
                {
                    HttpMethod = "Post",
                    UpdateTargetId = unobtrusiveAjaxOn == "true" ? null : containerName,
                    LoadingElementId =  loadingElementId,
                    OnSuccess = onSuccessName,
                    OnBegin = onBeginName,
                    OnFailure = onFailureName,
                    InsertionMode = InsertionMode.Replace
                },
                formHtmlAttributes))
            {
                htmlHelper.ViewContext.Writer.Write(beginTag);
                htmlHelper.ViewContext.Writer.Write(closeTag);

            }
            if (!MvcEnvironment.UnobtrusiveAjaxOn(htmlHelper))
            {
                htmlHelper.ViewContext.Writer.Write(
                 htmlHelper.AjaxSubmitEnabler(formName, false).ToString());
            }

        }

        public static MvcHtmlString DetailLink<TItem>(
            this HtmlHelper<TItem> htmlHelper,
            AjaxHelper ajax,
            string text,
            DetailType detailType,
            string controllerAction,
            string controllerName,
            object parameters,
            string loadingElementId,
            IDictionary<string, object> htmlAttributes=null)
        {
            if (ajax == null) throw (new ArgumentNullException("ajax"));
            if (text == null) throw (new ArgumentNullException("text"));
            if (controllerAction == null) throw (new ArgumentNullException("ControllerAction"));
            if (controllerName == null) throw (new ArgumentNullException("ControllerName"));
            string status = null;
            bool visible = true;
            if (htmlHelper.ViewData.ContainsKey("status")) status = htmlHelper.ViewData["status"] as string;
            string detail = "Display";
            if (status == "InsertItem")
            {
                if (detailType == DetailType.Edit)
                {
                    detail = "Insert";
                }
                else
                {
                    visible = false;
                }
            }
            else if (status == "Insert" || status == "Phantom")
            {
                visible = false;
            }
            else if(detailType == DetailType.Edit)
            {
                detail = "FirstEdit";
            }
            if (!visible) return MvcHtmlString.Create(string.Empty);
            string rootItemName=BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix);
            string baseName=rootItemName.Substring(0, rootItemName.LastIndexOf("___"));
            baseName=baseName.Substring(0, baseName.LastIndexOf("___"));

            string containerName= baseName+"_Container";
            string onSuccessName = baseName+"_OnSuccess";
            string onFailureName = baseName + "_OnFailure";
            string onBeginScript=
                MvcEnvironment.UnobtrusiveAjaxOn(htmlHelper) ?
                            "return OnBeginDetailForm('{0}', '{1}', '{2}', '{3}', null);" : "function() {{ return OnBeginDetailForm('{0}', '{1}', '{2}', '{3}', null); }}";
            return ajax.ActionLink(text, controllerAction, controllerName,
                parameters,
                new AjaxOptions()
                {
                    HttpMethod = "Get",
                    UpdateTargetId = containerName,
                    LoadingElementId = loadingElementId,
                    OnSuccess = onSuccessName,
                    OnBegin = 
                        string.Format(
                        onBeginScript,
                         baseName, detail, rootItemName, loadingElementId),         
                    OnFailure = onFailureName,
                    InsertionMode = InsertionMode.Replace
                },
                htmlAttributes);
        }

        public static void DetailFormFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            AjaxHelper<VM> ajax,
            Expression<Func<VM, IEnumerable<Tracker<TItem>>>> expression,
            ExternalContainerType externalContainerType,
            string postActionName,
            string postControllerName,
            string detailPrefix = null,
            string changedFieldCss = null,
            string deletedRecordCss = null,
            IDictionary<string, object> externalContainerHtmlattributes = null,
            string clientOnSuccessEdit = null,
            string clientOnFailureEdit = null,
            string clientOnSuccessDisplay = null,
            string clientOnFailureDisplay = null,
            string loadingElementId = null,
            Dialog detailDialog = null)
            where TItem : class, new()
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            DetailFormFor(
                htmlHelper,
                ajax,
                htmlHelper.ExtractFromModel(expression),
                externalContainerType,
                postActionName,
                postControllerName,
                detailPrefix,
                changedFieldCss,
                deletedRecordCss,
                externalContainerHtmlattributes,
                clientOnSuccessEdit,
                clientOnFailureEdit,
                clientOnSuccessDisplay,
                clientOnFailureDisplay,
                loadingElementId,
                detailDialog);
        }
    }
}

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
using MVCControlsToolkit.Controller;
using MVCControlsToolkit.Linq;
using MVCControlsToolkit.Controls.DataGrid;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
namespace MVCControlsToolkit.Controls
{
    public static class DataGridHelpers
    {

        static private string getDataButtonClass(DataButtonType dataButtonType)
        {
            switch(dataButtonType)
            {
                case DataButtonType.Cancel: return "DataButtonCancel";
                case DataButtonType.Delete: return "DataButtonDelete";
                case DataButtonType.Edit: return "DataButtonEdit";
                case DataButtonType.Undelete: return "DataButtonUndelete";
                case DataButtonType.ResetRow: return "DataButtonResetRow";
                default: return "DataButtonInsert";
            }
        }
        private static string templateSymbol="QS_23459_86{0}45_ZU";
        private static string buttonSchema =
            @"
            <input {0} />
            <script language='javascript' type='text/javascript'>
                $('#{4}').click(function() 
                {{
                    DataButton_Click('{1}', '{2}', '{3}');
                }});
               
            </script>
            ";
        private static string sortButtonSchema =
            @"
            <input {3} />
            <script language='javascript' type='text/javascript'>   
           
                $('#{2}').click(function() 
                {{
                    {0}_Sort('{1}', '{2}', false);
                }});
                $(document).ready(function()
                {{
                    setTimeout(function(){{{0}_Sort('{1}', '{2}', true);}}, 0); 
                }});
            </script>
            ";
        private static string linkSchema =
            @"
            <a {0}>{5}</a>
            <script language='javascript' type='text/javascript'>
                $('#{4}').click(function() 
                {{
                    DataButton_Click('{1}', '{2}', '{3}');
                }});
               
            </script>
            ";
        private static string sortLinkSchema =
            @"
            <a {3}>{4}</a>
            <script language='javascript' type='text/javascript'>
                 $('#{2}').click(function() 
                {{
                    {0}_Sort('{1}', '{2}', false);
                }});
                $(document).ready(function()
                {{
                    {0}_Sort('{1}', '{2}', true); 
                }});
            </script>
            ";
        private static string imgSchema =
            @"
            <img {0} />
            <script language='javascript' type='text/javascript'>
                $('#{4}').click(function() 
                {{
                    DataButton_Click('{1}', '{2}', '{3}');
                }});
               
            </script>
            ";
        private static string itemScriptTemplate=
            @"
            <script language='javascript' type='text/javascript'>
                var {4}_templateVars ='var {1}_Save=null; var {1}_SaveD=null; var {1}_SaveU=null; var {1}_SaveC=null; var {1}_Deleted=null; var {1}_SaveCurr=null; var {1}_Datagrid_Fields=null; var {2}_Var = {0};'; 
                var {4}_placeHolder = '{5}';
                var {4}_changedHidden = '{2}';
                var {4}_templateSymbol = /{6}/g;
                var {4}_lastIndex = '{7}';
                var {4}_lastVisibleIndex = '{7}';
                var {4}_minLastIndex = '{8}';
                var {4}_editHtml = null;
                var {4}_displayHtml = null;
                var {4}_allJavascript = null;
             " + "var {4}_templatePrepare = \"DataGrid_Prepare_Item('{1}', '{2}', {3}, '{4}');\";"+
            @"  $(document).ready(function()
                {{
                    DataGrid_Prepare_Template('{1}', '{2}', {3}, '{4}');
                }});
            </script>
            ";
        private static string itemScriptTemplateUnobtrusive =
           @"
            <script language='javascript' type='text/javascript'>
                var {4}_templateVars ='var {1}_Save=null; var {1}_SaveD=null; var {1}_SaveU=null; var {1}_SaveC=null; var {1}_Deleted=null; var {1}_SaveCurr=null; var {1}_Datagrid_Fields=null; var {2}_Var = {0};'; 
                var {4}_placeHolder = '{5}';
                var {4}_changedHidden = '{2}';
                var {4}_templateSymbol = /{6}/g;
                var {4}_lastIndex = '{7}';
                var {4}_lastVisibleIndex = '{7}';
                var {4}_minLastIndex = '{8}';
                var {4}_editHtml = null;
                var {4}_displayHtml = null;
                var {4}_allJavascript = null;
             " + "var {4}_templatePrepare = \"DataGrid_Prepare_Item('{1}', '{2}', {3}, '{4}');\";" +
            @"  $(document).ready(function()
                {{
                    DataGrid_Prepare_Template('{1}', '{2}', {3}, '{4}');
                }});
            </script>
            ";
        private static string itemScriptNoClient =
           @"
            <script language='javascript' type='text/javascript'>
                var {1}_Save=null;
                var {1}_SaveD=null;
                var {1}_SaveU=null;
                var {1}_SaveC=null;
                var {1}_Deleted=null;
                var {1}_SaveCurr=null;
                var {1}_Datagrid_Fields=null;
                var {2}_Var = {0};
                $(document).ready(function()
                {{"+
                    "setTimeout(\"DataGrid_Prepare_Item('{1}', '{2}', {3}, '{4}');\", 0);"+
            @"
                }});
            </script>
            ";
        private static string itemScriptClientUnobtrusive =
           @"
            <script language='javascript' type='text/javascript'>
                var {1}_Save=null;
                var {1}_SaveD=null;
                var {1}_SaveU=null;
                var {1}_SaveC=null;
                var {1}_Deleted=null;
                var {1}_SaveCurr=null;
                var {1}_Datagrid_Fields=null;
                var {2}_Var = {0};
                $(document).ready(function()
                {{" +
                    "setTimeout(\"DataGrid_Prepare_Item('{1}', '{2}', {3}, '{4}');\", 0);" +
            @"
                }});
            </script>
            ";
        private static string itemScriptClienStandard =
           @"
            <script language='javascript' type='text/javascript'>
                var {1}_Save=null;
                var {1}_SaveD=null;
                var {1}_SaveU=null;
                var {1}_SaveC=null;
                var {1}_Deleted=null;
                var {1}_SaveCurr=null;
                var {1}_Datagrid_Fields=null;
                var {2}_Var = {0};
                $(document).ready(function()
                {{" +
                    "setTimeout(\"DataGrid_Prepare_Item('{1}', '{2}', {3}, '{4}');\", 0);" +
            @"
                }});
            </script>
            ";
        private static string gridScript =
           @"
            <script language='javascript' type='text/javascript'>
                var {0}_AllNormal=null;
                var {0}_validationType = '{1}';
                var {0}_Css = '{2}';
                var {0}_AltCss = '{3}';
                var {0}_FatherItems = null;
                var {0}_FieldsToUpdate = {4};
            </script>
            ";
        private static string gridScriptInitialize =
            @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function()
                {{"+
                    "setTimeout(\"MvcControlsToolkit_DataGridApplyStyles('{0}');\", 0);"+
            @"
                }});
            </script>
            ";
        private static string gridScriptInitializeClientStandard =
            @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function()
                {{" +
                    "setTimeout(\"MvcControlsToolkit_DataGridApplyStyles('{0}');\", 0);" +
            @"
                }});
            </script>
            ";
        private static string sortScript =
            @"
            <script language='javascript' type='text/javascript'>
                var {1} = '';
                function {0}_Sort(field, buttonName, initialize){{
                    Sort_Handler(field, buttonName, initialize, {6}, {7}, '{1}', {2},'{3}','{4}','{5}', '{8}', {9});
                }}         
            </script>
            ";


       

        public static MvcHtmlString ThemedDataGridFor<M, TItem>(
            this HtmlHelper<M> htmlHelper,
            Expression<Func<M, IEnumerable<Tracker<TItem>>>> expression,
            GridFeatures gridFeatures,
            Columns<TItem> fields,
            Expression<Func<M, IEnumerable<KeyValuePair<LambdaExpression, OrderType>>>> orderExpression = null,
            Expression<Func<M, int>> page=null,
            Expression<Func<M, int>> prevPage=null,
            Expression<Func<M, int>> pageCount=null,
            Expression<Func<M, Expression<Func<TItem, bool>>>> filter=null,
            string title=null,
            string name = "DataGrid"
            )
            where TItem:class, new() 
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            
            if (fields==null || fields.Fields==null || fields.Fields.Count==0) throw(new ArgumentNullException("fields"));
            string themeName = ThemedControlsStrings.GetTheme();
            htmlHelper.ViewData["ThemeParams"] =
                new GridDescription
                {
                    ToShow = expression,
                    Fields = fields.Fields,
                    ToOrder= orderExpression,
                    Title = title,
                    HtmlHelper = htmlHelper,
                    Features = gridFeatures,
                    Page = page,
                    PrevPage = prevPage,
                    PageCount = pageCount,
                    Filter = filter
                };
            MvcHtmlString res;
            try
            {
                res = htmlHelper.Partial("Themes/" + themeName + "/"+name, htmlHelper.ViewData);
            }
            finally
            {
                htmlHelper.ViewData["ThemeParams"] = null;
            }
            return res;

        }



        public static MvcHtmlString ColumnNameForCollection<VM, Col, Field>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<Col>>> collection,
            Expression<Func<Col, Field>> field)
        {
            if (field == null) throw (new ArgumentNullException("field"));
            if (collection == null) throw (new ArgumentNullException("collection"));
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string Text = fieldName;

            PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(Col));

            DisplayAttribute[] display = pa[typeof(DisplayAttribute)] as DisplayAttribute[];
            if (display != null && display.Length != 0)
            {
                if (!string.IsNullOrWhiteSpace(display[0].GetShortName())) Text = display[0].GetShortName();
                else if (!string.IsNullOrWhiteSpace(display[0].GetName())) Text = display[0].GetName();
            }
            return MvcHtmlString.Create(htmlHelper.Encode(Text));

        }
        public static MvcHtmlString ColumnNameForTrackedCollection<VM, Col, Field>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<Tracker<Col>>>> collection,
            Expression<Func<Col, Field>> field)
            where Col:class, new()
        {
            if (field == null) throw (new ArgumentNullException("field"));
            if (collection == null) throw (new ArgumentNullException("collection"));
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string Text = fieldName;

            PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(Col));

            DisplayAttribute[] display = pa[typeof(DisplayAttribute)] as DisplayAttribute[];
            if (display != null && display.Length != 0)
            {
                if (!string.IsNullOrWhiteSpace(display[0].GetShortName())) Text = display[0].GetShortName();
                else if (!string.IsNullOrWhiteSpace(display[0].GetName())) Text = display[0].GetName();
            }
            return MvcHtmlString.Create(htmlHelper.Encode(Text));

        }
        public static MvcHtmlString ColumnNameFor<VM, FT>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, FT>> field)
        {
            if (field == null) throw (new ArgumentNullException("field"));
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string Text = fieldName;

            PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(VM));

            DisplayAttribute[] display = pa[typeof(DisplayAttribute)] as DisplayAttribute[];
            if (display != null && display.Length != 0)
            {
                if (!string.IsNullOrWhiteSpace(display[0].GetShortName())) Text = display[0].GetShortName();
                else if (!string.IsNullOrWhiteSpace(display[0].GetName())) Text = display[0].GetName();
            }
            return MvcHtmlString.Create(htmlHelper.Encode(Text));

        }

        public static MvcHtmlString SortButtonForCollection<VM, Col, Field>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<Col>>> collection,
            Expression<Func<Col, Field>> field,
            string Text = null,
            SortButtonStyle sortButtonStyle = SortButtonStyle.Link,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (field == null) throw (new ArgumentNullException("field"));
            if (collection == null) throw (new ArgumentNullException("collection"));
            string collectionName = ExpressionHelper.GetExpressionText(collection);
            string fieldNamePrefixed = BasicHtmlHelper.AddField(collectionName, ExpressionHelper.GetExpressionText(field));
            string buttonName = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldNamePrefixed));
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string fullCollectionName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(collectionName);
            if (Text == null)
            {
                Text = fieldName;
                PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(Col));

                DisplayAttribute[] display = pa[typeof(DisplayAttribute)] as DisplayAttribute[];
                if (display != null && display.Length != 0)
                {
                    if (!string.IsNullOrWhiteSpace(display[0].GetShortName())) Text = display[0].GetShortName();
                    else if (!string.IsNullOrWhiteSpace(display[0].GetName())) Text = display[0].GetName();
                }
            }
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            if (sortButtonStyle == SortButtonStyle.Button)
            {
                htmlAttributes["id"] = buttonName;
                htmlAttributes["type"] = "button";
                htmlAttributes["value"] = htmlHelper.Encode(Text);
                return
                    MvcHtmlString.Create(
                        string.Format(
                            sortButtonSchema,
                            BasicHtmlHelper.IdFromName(fullCollectionName),
                            fieldName,
                            buttonName,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes)));
            }
            else
            {
                htmlAttributes["id"] = buttonName;
                htmlAttributes["href"] = "javascript:void(0);";
                return
                    MvcHtmlString.Create(
                        string.Format(
                            sortLinkSchema,
                            BasicHtmlHelper.IdFromName(fullCollectionName),
                            fieldName,
                            buttonName,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes),
                            htmlHelper.Encode(Text)));

            }

        }
        public static MvcHtmlString SortButtonForTrackedCollection<VM, Col, Field>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<Tracker<Col>>>> collection,
            Expression<Func<Col, Field>> field,
            string Text = null,
            SortButtonStyle sortButtonStyle = SortButtonStyle.Link,
            IDictionary<string, object> htmlAttributes = null)
            where Col: class, new()
        {
            if (field == null) throw (new ArgumentNullException("field"));
            if (collection == null) throw (new ArgumentNullException("collection"));
            string collectionName = ExpressionHelper.GetExpressionText(collection);
            string fieldNamePrefixed = BasicHtmlHelper.AddField(collectionName, ExpressionHelper.GetExpressionText(field));
            string buttonName = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldNamePrefixed));
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string fullCollectionName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(collectionName);
            if (Text == null)
            {
                Text = fieldName;
                PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(Col));

                DisplayAttribute[] display = pa[typeof(DisplayAttribute)] as DisplayAttribute[];
                if (display != null && display.Length != 0)
                {
                    if (!string.IsNullOrWhiteSpace(display[0].GetShortName())) Text = display[0].GetShortName();
                    else if (!string.IsNullOrWhiteSpace(display[0].GetName())) Text = display[0].GetName();
                }
            }
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            if (sortButtonStyle == SortButtonStyle.Button)
            {
                htmlAttributes["id"] = buttonName;
                htmlAttributes["type"] = "button";
                htmlAttributes["value"] = htmlHelper.Encode(Text);
                return
                    MvcHtmlString.Create(
                        string.Format(
                            sortButtonSchema,
                            BasicHtmlHelper.IdFromName(fullCollectionName),
                            fieldName,
                            buttonName,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes)));
            }
            else
            {
                htmlAttributes["id"] = buttonName;
                htmlAttributes["href"] = "javascript:void(0);";
                return
                    MvcHtmlString.Create(
                        string.Format(
                            sortLinkSchema,
                            BasicHtmlHelper.IdFromName(fullCollectionName),
                            fieldName,
                            buttonName,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes),
                            htmlHelper.Encode(Text)));

            }

        }
        public static MvcHtmlString SortButtonFor<VM, FT>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, FT>> field,
            string Text = null,
            SortButtonStyle sortButtonStyle = SortButtonStyle.Link,
            IDictionary<string, object> htmlAttributes=null)
        {
            if (field == null) throw (new ArgumentNullException("field"));
            string fieldName = ExpressionHelper.GetExpressionText(field);
            string buttonName = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName));

            if (Text == null)
            {
                Text = fieldName;
                PropertyAccessor pa = new PropertyAccessor(fieldName, typeof(VM));
                
                DisplayAttribute[] display = pa[typeof(DisplayAttribute)] as DisplayAttribute[];
                if (display != null && display.Length != 0)
                {
                    if (!string.IsNullOrWhiteSpace(display[0].GetShortName())) Text = display[0].GetShortName();
                    else if (!string.IsNullOrWhiteSpace(display[0].GetName())) Text = display[0].GetName(); 
                }
            }
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            if (sortButtonStyle == SortButtonStyle.Button)
            {
                htmlAttributes["id"] = buttonName;
                htmlAttributes["type"] = "button";
                htmlAttributes["value"] = htmlHelper.Encode(Text);
                return
                    MvcHtmlString.Create(
                        string.Format(
                            sortButtonSchema,
                            BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix),
                            fieldName,
                            buttonName,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes)));
            }
            else
            {
                htmlAttributes["id"] = buttonName;
                htmlAttributes["href"] = "javascript:void(0);";
                return
                    MvcHtmlString.Create(
                        string.Format(
                            sortLinkSchema,
                            BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix),
                            fieldName,
                            buttonName,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes),
                            htmlHelper.Encode(Text)));
                
            }

        }


        public static MvcHtmlString DataButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            DataButtonType dataButtonType,
            string Text,
            IDictionary<string, object> htmlAttributes)
        {
            if (Text == null) Text = string.Empty;
            if (htmlAttributes==null) htmlAttributes= new Dictionary<string, object>();
            string buttonId=BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, getDataButtonClass(dataButtonType)));
            htmlAttributes["id"] = buttonId;
            BasicHtmlHelper.AddClass(htmlAttributes, getDataButtonClass(dataButtonType));
            htmlAttributes["type"] = "button";
            htmlAttributes["value"] = htmlHelper.Encode(Text);
            
            int startChanged = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.LastIndexOf(".Value");
            string changedName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Substring(0, startChanged) + ".Changed";
            return MvcHtmlString.Create(
                string.Format(buttonSchema,
                    BasicHtmlHelper.GetAttributesString(htmlAttributes),
                    BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix),
                    BasicHtmlHelper.IdFromName(changedName),
                    getDataButtonClass(dataButtonType),
                    buttonId));

        }
        public static MvcHtmlString ImgDataButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            DataButtonType dataButtonType,
            string source,
            IDictionary<string, object> htmlAttributes)
        {
            if (source == null) source = string.Empty;
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            string buttonId = BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, getDataButtonClass(dataButtonType)));
            htmlAttributes["id"] = buttonId;
            BasicHtmlHelper.AddClass(htmlAttributes, getDataButtonClass(dataButtonType));
            htmlAttributes["src"] = source;
            BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "cursor", "pointer"); 
            int startChanged = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.LastIndexOf(".Value");
            string changedName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Substring(0, startChanged) + ".Changed";
            return MvcHtmlString.Create(
                string.Format(imgSchema,
                    BasicHtmlHelper.GetAttributesString(htmlAttributes),
                    BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix),
                    BasicHtmlHelper.IdFromName(changedName),
                    getDataButtonClass(dataButtonType),
                    buttonId));

        }
        public static MvcHtmlString LinkDataButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            DataButtonType dataButtonType,
            string Text,
            IDictionary<string, object> htmlAttributes)
        {
            if (Text == null) Text = string.Empty;
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            string buttonId = BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, getDataButtonClass(dataButtonType)));
            htmlAttributes["id"] = buttonId;
            BasicHtmlHelper.AddClass(htmlAttributes, getDataButtonClass(dataButtonType));
            htmlAttributes["href"] = "javascript:void(0);";
            int startChanged = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.LastIndexOf(".Value");
            string changedName = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix.Substring(0, startChanged) + ".Changed";
            return MvcHtmlString.Create(
                string.Format(linkSchema,
                    BasicHtmlHelper.GetAttributesString(htmlAttributes),
                    BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix),
                    BasicHtmlHelper.IdFromName(changedName),
                    getDataButtonClass(dataButtonType),
                    buttonId,
                    htmlHelper.Encode(Text)));

        }

        private static MvcHtmlString IEnableSortingFor<VM, C, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<C> collection,
            RenderInfo<IEnumerable<KeyValuePair<LambdaExpression, OrderType>>> sorting,
            string cssClassNoSort, string cssClassAscending, string cssClassDescending,
            bool causePostback = false,
            string clientOrderChanged = null,
            RenderInfo<IEnumerable<KeyValuePair<LambdaExpression, OrderType>>> previousSorting = null,
            Expression<Func<VM, int>> page = null,
            bool oneColumnSorting=false)
            
        {
            if (collection == null) throw (new ArgumentNullException("collection"));
            if (sorting == null) throw (new ArgumentNullException("sorting"));

            SortInfo<TItem> si = new SortInfo<TItem>();
            si.ImportFromModel(sorting.Model);

            StringBuilder sb = new StringBuilder();
            string sortingPartialRendering = sorting.PartialRendering;
            if (!string.IsNullOrWhiteSpace(sortingPartialRendering))
                sb.Append(sortingPartialRendering);
            sb.Append(BasicHtmlHelper.RenderDisplayInfo(
                htmlHelper,
                typeof(SortInfo<TItem>),
                sorting.PartialPrefix));
            sb.Append(htmlHelper.GenericInput(
                InputType.Hidden,
                BasicHtmlHelper.AddField(sorting.PartialPrefix, "$.SortInfoAsString"),
                si.SortInfoAsString,
                null));
            if (previousSorting != null)
            {

                si = new SortInfo<TItem>();
                si.ImportFromModel(sorting.Model);

                string previousSortingPartialRendering=previousSorting.PartialRendering;
                if (!string.IsNullOrWhiteSpace((previousSortingPartialRendering)))
                    sb.Append((previousSortingPartialRendering));
                sb.Append(BasicHtmlHelper.RenderDisplayInfo(
                    htmlHelper,
                    typeof(SortInfo<TItem>),
                    previousSorting.PartialPrefix));
                sb.Append(htmlHelper.GenericInput(
                    InputType.Hidden,
                    BasicHtmlHelper.AddField(previousSorting.PartialPrefix, "$.SortInfoAsString"),
                    si.SortInfoAsString,
                    null));
            }
            string pageField = "null";
            string javascriptCausePostback = causePostback ? "true" : "false";
            string javascriptOrderChanged = string.IsNullOrWhiteSpace(clientOrderChanged) ? "null" : "'" + clientOrderChanged + "'";
            if (page != null)
            {
                pageField = "'" +
                    BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(page)))
                    + "'";
            }
            string validationType = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }
            sb.Append(
                string.Format(sortScript,
                    BasicHtmlHelper.IdFromName(collection.Prefix),
                    BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(sorting.Prefix, "$.SortInfoAsString")),
                    pageField,
                    cssClassNoSort, cssClassAscending, cssClassDescending, 
                    javascriptCausePostback, javascriptOrderChanged, validationType,
                    oneColumnSorting ? "true": "false"));
            return
                MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString EnableSortingFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<Tracker<TItem>>> collection,
            RenderInfo<IEnumerable<KeyValuePair<LambdaExpression, OrderType>>> sorting,
            string cssClassNoSort, string cssClassAscending, string cssClassDescending, 
            bool causePostback = false,
            string clientOrderChanged=null,
            RenderInfo<IEnumerable<KeyValuePair<LambdaExpression, OrderType>>> previousSorting = null,
            Expression<Func<VM, int>> page=null,
            bool oneColumnSorting=false)
            where TItem : class, new() 
        {
            if (collection == null) throw (new ArgumentNullException("collection"));
            if (sorting == null) throw (new ArgumentNullException("sorting"));


            return
                htmlHelper.IEnableSortingFor<VM, IEnumerable<Tracker<TItem>>, TItem>(
                collection,
                sorting,
                cssClassNoSort,
                cssClassAscending,
                cssClassDescending,
                causePostback,
                clientOrderChanged,
                previousSorting,
                page,
                oneColumnSorting);
                
        }
        public static MvcHtmlString EnableSortingNoTrackFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<TItem>> collection,
            RenderInfo<IEnumerable<KeyValuePair<LambdaExpression, OrderType>>> sorting,
            string cssClassNoSort, string cssClassAscending, string cssClassDescending,
            bool causePostback = false,
            string clientOrderChanged = null,
            RenderInfo<IEnumerable<KeyValuePair<LambdaExpression, OrderType>>> previousSorting = null,
            Expression<Func<VM, int>> page = null,
            bool oneColumnSorting=false)
            
        {
            if (collection == null) throw (new ArgumentNullException("collection"));
            if (sorting == null) throw (new ArgumentNullException("sorting"));


            return
                htmlHelper.IEnableSortingFor<VM, IEnumerable<TItem>, TItem>(
                collection,
                sorting,
                cssClassNoSort,
                cssClassAscending,
                cssClassDescending,
                causePostback,
                clientOrderChanged,
                previousSorting,
                page,
                oneColumnSorting);

        }

        public static MvcHtmlString EnableSortingNoTrackFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<TItem>>> collection,
            Expression<Func<VM, IEnumerable<KeyValuePair<LambdaExpression, OrderType>>>> sorting,
            string cssClassNoSort, string cssClassAscending, string cssClassDescending,
            bool causePostback = false,
            string clientOrderChanged = null,
            Expression<Func<VM, IEnumerable<KeyValuePair<LambdaExpression, OrderType>>>> previousSorting = null,
            Expression<Func<VM, int>> page = null,
            bool oneColumnSorting=false)
            
        {
            if (collection == null) throw (new ArgumentNullException("collection"));
            if (sorting == null) throw (new ArgumentNullException("sorting"));
            return
                EnableSortingNoTrackFor(htmlHelper,
                htmlHelper.ExtractFromModel(collection),
                htmlHelper.ExtractFromModel(sorting),
                cssClassNoSort,
                cssClassAscending,
                cssClassDescending,
                causePostback,
                clientOrderChanged,
                previousSorting == null ? null : htmlHelper.ExtractFromModel(previousSorting),
                page,
                oneColumnSorting);
        }

        public static MvcHtmlString EnableSortingFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM,IEnumerable<Tracker<TItem>>>> collection,
            Expression<Func<VM,IEnumerable<KeyValuePair<LambdaExpression, OrderType>>>> sorting,
            string cssClassNoSort, string cssClassAscending, string cssClassDescending,
            bool causePostback=false,
            string clientOrderChanged=null,
            Expression<Func<VM, IEnumerable<KeyValuePair<LambdaExpression, OrderType>>>> previousSorting = null,
            Expression<Func<VM, int>> page = null,
            bool oneColumnSorting = false)
            where TItem : class, new() 
        {
            if (collection == null) throw (new ArgumentNullException("collection"));
            if (sorting == null) throw (new ArgumentNullException("sorting"));
            return
                EnableSortingFor(htmlHelper,
                htmlHelper.ExtractFromModel(collection),
                htmlHelper.ExtractFromModel(sorting),
                cssClassNoSort,
                cssClassAscending,
                cssClassDescending,
                causePostback,
                clientOrderChanged,
                previousSorting == null ? null : htmlHelper.ExtractFromModel(previousSorting),
                page,
                oneColumnSorting);
        }

        private static Type getItemType<VM, TItem>(Tracker<TItem> tracker, HtmlHelper<VM> htmlHelper)
            where TItem: class, new()
        {
            Type itemType = null;
            if (tracker.Value != null) itemType = tracker.Value.GetType();
            else if (tracker.OldValue != null) itemType = tracker.OldValue.GetType();
            if (itemType == null || itemType == typeof(TItem)) return null;
            return typeof(DataGridItem<TItem, VM>).GetGenericTypeDefinition().MakeGenericType(itemType, typeof(VM));
        }
        private static MvcHtmlString DataGridFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<Tracker<TItem>>> renderInfo,
            ItemContainerType itemContainerType,
            object editTemplate,
            object displayTemplate,
            object gridTemplate=null,
            object addDisplayTemplate=null,
            object deletedTemplate=null,
            object changedTemplate=null,
            IDictionary<string, object> editContainerAttributes=null,
            IDictionary<string, object> displayContainerAttributes=null,
            IDictionary<string, object> deletedContainerAttributes = null,
            IDictionary<string, object> changedContainerAttributes = null,
            Func<TItem, int, string> getSeparator=null,
            Func<TItem, int, string> getDynamicContainer=null,
            bool enableMultipleInsert = true,
            string itemCss=null,
            string altItemCss=null,
            FieldsToTrack toTrack=null
            )
            where TItem:class, new() 
        {
            
            if (displayTemplate == null) throw (new ArgumentNullException("displayTemplateName"));
            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));
            
            if (renderInfo.Model == null) renderInfo.Model=new List<Tracker<TItem>>();
            string itemScript = null;
            string templateScript = null;
            bool templateEnabled = false;
            string validationType = null;
            if (string.IsNullOrWhiteSpace(itemCss)) itemCss = string.Empty;
            if(string.IsNullOrWhiteSpace(altItemCss)) altItemCss = string.Empty;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: 
                    itemScript = itemScriptClienStandard; validationType="StandardClient"; break;
                case ValidationType.UnobtrusiveClient: 
                    itemScript = itemScriptClientUnobtrusive; templateScript = itemScriptTemplateUnobtrusive; 
                    templateEnabled = true; validationType = "UnobtrusiveClient"; break;
                default: itemScript = 
                    itemScriptNoClient; templateScript = itemScriptTemplate; templateEnabled = true; 
                    validationType = "Server"; break;
            }
            templateEnabled = templateEnabled && enableMultipleInsert;
            PropertyInfo[] propertiesTostore = null;
            string fieldsToUpdate = null;
            if (toTrack == null)
            {
                propertiesTostore = BasicHtmlHelper.GetPropertiesForInput(typeof(TItem));
                fieldsToUpdate = "null";
            }
            else
            {
                fieldsToUpdate = toTrack.FieldsToUpdate();
            }

            StringBuilder sb = new StringBuilder();
            htmlHelper.ViewData[renderInfo.Prefix + "_Rendering"] = "DataGrid";
            
            StringBuilder sbInitRender = new StringBuilder();
            string renderInfoPartialRendering=renderInfo.PartialRendering;
            if (!string.IsNullOrWhiteSpace(renderInfoPartialRendering))
                sbInitRender.Append(renderInfoPartialRendering);
            sbInitRender.Append(string.Format(gridScript, BasicHtmlHelper.IdFromName(renderInfo.Prefix), validationType, itemCss, altItemCss, fieldsToUpdate));
            int i = 0;
            int count = 0;
            foreach(var x in renderInfo.Model) count++;
            count--;
            bool lastInsert = false;
            int lastInsertPosition = 0;
            int normalElementCount=0;
            int insertClosedPosition = -2;
            foreach (Tracker<TItem> tracker in renderInfo.Model)
            {
                bool normal = false;
                if (tracker == null) continue;
                DataGridItem<TItem, VM> item = new DataGridItem<TItem, VM>();
                item.SetPropertiesToStore(propertiesTostore);
                item.SetFieldsToStore(toTrack);
                item.ImportFromModel(tracker, null, null);
                string prefix = renderInfo.Prefix;
                string partialPrefix = renderInfo.PartialPrefix;
                sbInitRender.Append(
                    BasicHtmlHelper.RenderUpdateInfo<Tracker<TItem>>(htmlHelper, item, ref partialPrefix, new string[0], null, getItemType(tracker, htmlHelper)));
                prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                sbInitRender.Append(
                    item.GetChangedAndOldValuesRendering(partialPrefix, htmlHelper));
                sbInitRender.Append(
                    string.Format(
                        itemScript,
                        tracker.Changed ? "false" : "true",
                        BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Value")),
                        BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Changed")), tracker.Value == null ? "true" : "false", BasicHtmlHelper.IdFromName(renderInfo.Prefix)));
                if (tracker.Value == null)
                {
                    if (tracker.OldValue == null)
                    {
                        item.SetStatus("Phantom");
                    }
                    else
                    {
                        item.SetStatus("Deleted");
                    }
                }
                else
                {
                    if (tracker.OldValue == null)
                    {
                        item.SetStatus("Insert");
                    }
                    else
                    {
                        item.SetStatus("Edited");
                    }
                }
                if (item.Item.OldValue == null)
                {
                    if (!lastInsert)
                    {
                        lastInsert = true;
                        lastInsertPosition = normalElementCount;
                    }
                    if (item.Item.Value == null)
                    {
                        insertClosedPosition = normalElementCount;
                    }
                }
                else
                {
                    lastInsert = false;
                    lastInsertPosition = normalElementCount+1;
                }
                normalElementCount++;
                if (tracker.Value != null)
                {
                    Type t = typeof(TItem);

                    if (item.Item.Changed && item.Item.OldValue == null)
                        sb.Append(item.GetDisplayItem(prefix, htmlHelper, addDisplayTemplate, itemContainerType, displayContainerAttributes, i, normal, getDynamicContainer));
                    else
                        sb.Append(item.GetDisplayItem(prefix, htmlHelper, displayTemplate, itemContainerType, displayContainerAttributes, i, normal, getDynamicContainer));
                    if (deletedTemplate != null)
                        sb.Append(item.GetDeleteItem(prefix, htmlHelper, deletedTemplate, itemContainerType, deletedContainerAttributes, i, normal, getDynamicContainer));
                    if (changedTemplate != null)
                        sb.Append(item.GetChangedItem(prefix, htmlHelper, changedTemplate, itemContainerType, changedContainerAttributes, i, normal, getDynamicContainer));
                    if (editTemplate != null)
                    {
                        sb.Append(item.GetEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, i, normal, getDynamicContainer));
                        if (tracker.Changed)
                            sb.Append(item.GetOldEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, i, normal, getDynamicContainer));
                    }
                    if (i < count && getSeparator != null)
                    {
                        sb.Append(getSeparator(tracker.OldValue, i));
                    }

                    i++;
                }
                else
                {
                    sb.Append(item.GetDisplayItem(prefix, htmlHelper, displayTemplate, itemContainerType, displayContainerAttributes, i, normal, getDynamicContainer));
                    if (changedTemplate != null)
                        sb.Append(item.GetChangedItem(prefix, htmlHelper, changedTemplate, itemContainerType, changedContainerAttributes, i, normal, getDynamicContainer));
                    if (editTemplate != null)
                    {
                        sb.Append(item.GetEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, i, normal, getDynamicContainer));
                        if (tracker.Changed)
                            sb.Append(item.GetOldEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, i, normal, getDynamicContainer));
                    }
                    if (deletedTemplate != null)
                    {
                        sb.Append(item.GetDeleteItem(prefix, htmlHelper, deletedTemplate, itemContainerType, deletedContainerAttributes, i, normal, getDynamicContainer));
                        if (i < count && getSeparator != null)
                        {
                            sb.Append(getSeparator(tracker.OldValue, i));
                        }
                        i++;
                    }
                }
            }
            if (addDisplayTemplate != null && editTemplate != null &&  insertClosedPosition != normalElementCount-1)
            {
                DataGridItem<TItem, VM> item = new DataGridItem<TItem, VM>();
                item.Item = new Tracker<TItem>();
                item.Item.Value = new TItem();
                item.SetPropertiesToStore(propertiesTostore);
                item.SetFieldsToStore(toTrack);
                item.SetStatus("InsertItem");
                string prefix = renderInfo.Prefix;
                string partialPrefix = renderInfo.PartialPrefix;
                sbInitRender.Append(
                    BasicHtmlHelper.RenderUpdateInfo<Tracker<TItem>>(htmlHelper, item, ref partialPrefix, new string[0]));
                prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                sbInitRender.Append(
                    item.GetChangedAndOldValuesRendering(partialPrefix, htmlHelper));
                sbInitRender.Append(
                    string.Format(
                        itemScript,
                        item.Item.Changed ? "false" : "true",
                        BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Value")),
                        BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Changed")), "false", BasicHtmlHelper.IdFromName(renderInfo.Prefix)));
                sb.Append(item.GetDisplayItem(prefix, htmlHelper, addDisplayTemplate, itemContainerType, displayContainerAttributes, -1, false, getDynamicContainer));
                sb.Append(item.GetEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, -1, false, getDynamicContainer));
            }
            if (templateEnabled && addDisplayTemplate != null && editTemplate != null)
            {
                DataGridItem<TItem, VM> item = new DataGridItem<TItem, VM>();
                item.Item = new Tracker<TItem>();
                item.Item.Value = new TItem();
                item.SetPropertiesToStore(propertiesTostore);
                item.SetFieldsToStore(toTrack);
                item.SetStatus("InsertItem");
                string prefix = renderInfo.Prefix;
                string partialPrefix = renderInfo.PartialPrefix;
                //One needs unique template symbols because grids can be nested
                string myTemplateSymbol = BasicHtmlHelper.GetUniqueSymbol(htmlHelper, templateSymbol);
                
                sbInitRender.Append(
                    BasicHtmlHelper.RenderUpdateInfo<Tracker<TItem>>(htmlHelper, item, ref partialPrefix, new string[0], myTemplateSymbol));

                prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
                sbInitRender.Append(
                    item.GetChangedAndOldValuesRendering(partialPrefix, htmlHelper));
                sbInitRender.Append(
                    string.Format(
                        templateScript,
                        item.Item.Changed ? "false" : "true",
                        BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Value")),
                        BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Changed")), "false",
                        BasicHtmlHelper.IdFromName(renderInfo.Prefix), BasicHtmlHelper.IdFromName(prefix), myTemplateSymbol, count+1, lastInsertPosition));
                sb.Append(string.Format("<span id='{0}' style='display:none' class='MVCCT_EncodedTemplate'>", 
                    BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Value.Display.Container"))));
                sb.Append(htmlHelper.Encode(item.GetDisplayItem(prefix, htmlHelper, addDisplayTemplate, itemContainerType, displayContainerAttributes, -1, false, getDynamicContainer)));
                sb.Append("</span>");
                sb.Append(string.Format("<span id='{0}' style='display:none' class='MVCCT_EncodedTemplate'>",
                    BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Value.Edit.Container"))));
                sb.Append(htmlHelper.Encode(item.GetEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, -1, false, getDynamicContainer)));
                sb.Append("</span>");
            }
            if (validationType == "StandardClient")
            {
                sbInitRender.Append(string.Format(gridScriptInitializeClientStandard, BasicHtmlHelper.IdFromName(renderInfo.Prefix)));
            }
            else
            {
                sbInitRender.Append(string.Format(gridScriptInitialize, BasicHtmlHelper.IdFromName(renderInfo.Prefix)));
            }
            if (gridTemplate == null)
            {
                sbInitRender.Insert(0, sb.ToString());
                return MvcHtmlString.Create(sbInitRender.ToString());
            }
            else
            {
                ViewDataDictionary<TItem> dataDictionary = new ViewDataDictionary<TItem>(new TItem());
                dataDictionary["Content"] = MvcHtmlString.Create(sb.ToString());
                dataDictionary.TemplateInfo.HtmlFieldPrefix = renderInfo.Prefix;
                sbInitRender.Insert(0, new TemplateInvoker<TItem>(gridTemplate).Invoke<VM>(htmlHelper, dataDictionary));
                return MvcHtmlString.Create(sbInitRender.ToString());
                    
            }
        }
        private static MvcHtmlString DataGridAddItemFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            RenderInfo<IEnumerable<Tracker<TItem>>> renderInfo,
            ItemContainerType itemContainerType,
            object editTemplate,
            object addDisplayTemplate,
            IDictionary<string, object> editContainerAttributes = null,
            IDictionary<string, object> displayContainerAttributes = null,
            Func<TItem, int, string> getDynamicContainer = null
            )
            where TItem : class, new()
        {
            if (editTemplate == null) throw (new ArgumentNullException("editTemplateName"));
            if (addDisplayTemplate == null) throw (new ArgumentNullException("addDisplayTemplateName"));
            if (renderInfo == null) throw (new ArgumentNullException("renderInfo"));

            string itemScript = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: itemScript = itemScriptClienStandard; break;
                case ValidationType.UnobtrusiveClient: itemScript = itemScriptClientUnobtrusive; break;
                default: itemScript = itemScriptNoClient; break;
            }
            
            PropertyInfo[] propertiesTostore = BasicHtmlHelper.GetPropertiesForInput(typeof(TItem));

            StringBuilder sb = new StringBuilder();
            string renderInfoPartialRendering = renderInfo.PartialRendering;
            if (!string.IsNullOrWhiteSpace(renderInfoPartialRendering))
                 sb.Append(renderInfoPartialRendering);

            
            DataGridItem<TItem, VM> item = new DataGridItem<TItem, VM>();
            item.Item = new Tracker<TItem>();
            item.Item.Value = new TItem();
            item.SetPropertiesToStore(propertiesTostore);
            item.SetStatus("InsertItem");
            string prefix = renderInfo.Prefix;
            string partialPrefix = renderInfo.PartialPrefix;
            sb.Append(
                BasicHtmlHelper.RenderUpdateInfo<Tracker<TItem>>(htmlHelper, item, ref prefix, new string[0]));
            prefix = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialPrefix);
            sb.Append(
                item.GetChangedAndOldValuesRendering(prefix, htmlHelper));
            sb.Append(
                   string.Format(
                       itemScript,
                       item.Item.Changed ? "false" : "true",
                       BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Value")),
                       BasicHtmlHelper.IdFromName(BasicHtmlHelper.AddField(prefix, "$.Item.Changed")), "false", BasicHtmlHelper.IdFromName(renderInfo.Prefix)));
            sb.Append(item.GetDisplayItem(prefix, htmlHelper, addDisplayTemplate, itemContainerType, displayContainerAttributes, -1, true, getDynamicContainer));
            sb.Append(item.GetEditItem(prefix, htmlHelper, editTemplate, itemContainerType, editContainerAttributes, -1,true,  getDynamicContainer));
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString DataGridFor<VM, TItem>(
           this HtmlHelper<VM> htmlHelper,
           Expression<Func<VM, IEnumerable<Tracker<TItem>>>> expression,
           ItemContainerType itemContainerType,
           object editTemplate,
           object displayTemplate,
           object gridTemplate=null,
           object addDisplayTemplate=null,
            object deletedTemplate = null,
            object changedTemplate=null,
           IDictionary<string, object> editContainerAttributes = null,
           IDictionary<string, object> displayContainerAttributes = null,
           IDictionary<string, object> deletedContainerAttributes = null,
           IDictionary<string, object> changedContainerAttributes = null,
           Func<TItem, int, string> getSeparator = null,
           Func<TItem, int, string> getDynamicContainer = null,
           bool enableMultipleInsert = true,
           string itemCss = null,
           string altItemCss = null,
           FieldsToTrack toTrack=null
           )
           where TItem : class, new()
        {
            
            if (displayTemplate == null) throw (new ArgumentNullException("displayTemplateName"));
            if (expression == null) throw (new ArgumentNullException("expression"));
            return
                htmlHelper.DataGridFor(
                    htmlHelper.ExtractFromModel(expression),
                    itemContainerType,
                    editTemplate,
                    displayTemplate,
                    gridTemplate,
                    addDisplayTemplate,
                    deletedTemplate,
                    changedTemplate,
                    editContainerAttributes,
                    displayContainerAttributes,
                    deletedContainerAttributes,
                    changedContainerAttributes,
                    getSeparator,
                    getDynamicContainer,
                    enableMultipleInsert,
                    itemCss,
                    altItemCss,
                    toTrack);

        }

        public static MvcHtmlString DataGridAddItemFor<VM, TItem>(
            this HtmlHelper<VM> htmlHelper,
            Expression<Func<VM, IEnumerable<Tracker<TItem>>>> expression,
            ItemContainerType itemContainerType,
            object editTemplate,
            object addDisplayTemplate,
            IDictionary<string, object> editContainerAttributes = null,
            IDictionary<string, object> displayContainerAttributes = null,
            Func<TItem, int, string> getDynamicContainer = null
            )
            where TItem : class, new()
        {
            if (editTemplate == null) throw (new ArgumentNullException("editTemplateName"));
            if (addDisplayTemplate == null) throw (new ArgumentNullException("addDisplayTemplateName"));
            if (expression == null) throw (new ArgumentNullException("expression"));

            return htmlHelper.DataGridAddItemFor<VM, TItem>(
                htmlHelper.ExtractFromModel(expression),
                    itemContainerType,
                    editTemplate,
                    addDisplayTemplate,
                    editContainerAttributes,
                    displayContainerAttributes,
                    getDynamicContainer);
        }
    }
}

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
using MVCControlsToolkit.Core;


namespace MVCControlsToolkit.Controls
{
    
    public enum PageButtonType{Next, Previous, First, Last, GoTo};
    public enum PageButtonStyle{Button, Link, Image};
    public class Pager<VM>
    {
        private static string templateSymbol = "QR1_23459_8645_ZU";
        private static string buttonSchema =
            @"
            <input {0} />
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                    PageButton_Click('{1}', {2}, {4}, '{5}', '{6}');
                }});
               
            </script>
            ";
        private static string linkSchema =
            @"
            <a {0}>{4}</a>
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                    PageButton_Click('{1}', {2}, {5}, '{6}', '{7}');
                }});
               
            </script>
            ";
        private static string linkSchemaGet =
            @"
            <a {0}>{1}</a>
            
            ";
        private static string linkSchemaDisabled =
            @"
            <span {0}>{4}</span>
            
            ";
        private static string imgSchema =
            @"
            <img {0} />
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                     PageButton_Click('{1}', {2}, {4}, '{5}', '{6}');
                }});
               
            </script>
            ";

        private static string imgSchemaDisabled =
        @"
            <img {0} />
            
            ";
        
        private int currPage;
        private int? prevPage;
        private int? totPages;
        string fieldName;
        string fieldPartialName;
        string prevFieldName;
        HtmlHelper<VM> htmlHelper;
        bool pageFieldRendered;

        string controllerName;
        string actionName;
        string parameterName;
        string targetIdName;
        string goTextName;
        object otherParameters=null;
        string protocol=null;
        string hostname=null;
        string fragment = null;
        string routeName=null;
        string validationType = null;

        public Pager(HtmlHelper<VM> htmlHelper, string fieldName, string prevFieldName, int currPage, int? prevPage, int? totPages, string fieldPartialName)
        {
            this.currPage = currPage;
            this.prevPage = prevPage;
            this.totPages = totPages;
            this.fieldName = fieldName;
            this.fieldPartialName = fieldPartialName;
            this.prevFieldName = prevFieldName;
            pageFieldRendered = false;
            this.htmlHelper = htmlHelper;
            this.targetIdName = string.Empty;
            goTextName = fieldPartialName + "_gotext";
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }
            
        }
        public Pager(HtmlHelper<VM> htmlHelper, int currPage, int? totPages,
            string parameterName, string actionName, string targetIdName, string controllerName=null, 
            object otherParameters=null,
            string routeName=null,
            string protocol=null,
            string hostname=null, 
            string fragment=null)
        {
            this.currPage = currPage;
            this.totPages = totPages;
            this.fieldName = string.Empty;
            this.fieldPartialName = string.Empty;
            pageFieldRendered = false;
            this.htmlHelper = htmlHelper;

            this.controllerName = controllerName;
            this.actionName = actionName;
            this.parameterName = parameterName;
            this.targetIdName = targetIdName;

            this.protocol = protocol;
            this.hostname = hostname;
            this.fragment = fragment;
            this.routeName = routeName;

            this.otherParameters = otherParameters;
            if (string.IsNullOrWhiteSpace(controllerName))
                goTextName = "CurrentController_" + actionName + "_" + parameterName + "_gotext";
            else
                goTextName = controllerName + "_" + actionName + "_" + parameterName + "_gotext";
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }

        }
        protected string RenderPageField()
        {
            if (pageFieldRendered) return string.Empty;
            pageFieldRendered = true;
            if (prevPage != null && prevPage.HasValue)
            {
                string res = string.Format(@"<input type='hidden' value='{0}' id='{1}' name='{2}' /><input type='hidden' value='{3}' id='{4}'  name='{5}'/>",
                    prevPage.Value, BasicHtmlHelper.IdFromName(prevFieldName), prevFieldName, currPage, BasicHtmlHelper.IdFromName(fieldName), fieldName);
                // htmlHelper.Hidden(prevFieldName, prevPage.Value).ToString() + htmlHelper.Hidden(fieldName, currPage).ToString();
                return res;


            }
            else if (!string.IsNullOrEmpty(fieldName))
            {
                return
                     string.Format(@"<input type='hidden' value='{0}' id='{1}'  name='{2}'/>", currPage, BasicHtmlHelper.IdFromName(fieldName), fieldName);
            }
            else
                return string.Empty;
        }
        private string getPageButtonClass(PageButtonType pageButtonType)
        {
            switch (pageButtonType)
            {
                case PageButtonType.First: return "PageButtonFirst";
                case PageButtonType.Last: return "PageButtonLast";
                case PageButtonType.Next: return "PageButtonNext";
                case PageButtonType.Previous: return "PageButtonPrevious";
                case PageButtonType.GoTo: return "PageButtonGoTo";
                default: return string.Empty; ;
            }
        }
        private string pageString(int page)
        {
            if (page < 0) return string.Format("$('#{0}').val()||'{1}'", BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(goTextName)), currPage);
            else return string.Format("'{0}'", page);
        }
        private string clientUrl(int page, string serverUrl)
        {
            if (page < 0) return string.Format("'{1}'.replace(/{2}/g, $('#{0}').val()||'{3}')", BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(goTextName)), serverUrl, templateSymbol, currPage);
            else return string.Format("'{0}'", serverUrl);
        }
        protected string RenderPageButton(string name, string textOrUrl, int page,
            PageButtonStyle pageButtonStyle, IDictionary<string, object> htmlAttributes = null, bool encoded = true)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();

            string buttonId = null;

            if (!string.IsNullOrEmpty(fieldName))
                buttonId = BasicHtmlHelper.IdFromName(
                 BasicHtmlHelper.AddField(fieldName, name));
            else
            {
                if (string.IsNullOrWhiteSpace(controllerName))
                    buttonId = BasicHtmlHelper.IdFromName(
                     BasicHtmlHelper.AddField("CurrentController_" + actionName + "_" + parameterName, name));
                else
                    buttonId = BasicHtmlHelper.IdFromName(
                     BasicHtmlHelper.AddField(controllerName + "_" + actionName + "_" + parameterName, name));
            }
            
            string pageUrl = string.Empty;
            if (parameterName != null)
            {
                System.Web.Routing.RouteValueDictionary routeDictionary=null;
                if (otherParameters != null)
                {
                    routeDictionary=otherParameters as System.Web.Routing.RouteValueDictionary;
                    if (routeDictionary == null) routeDictionary=new System.Web.Routing.RouteValueDictionary(otherParameters);
                }
                else
                {
                    routeDictionary=new System.Web.Routing.RouteValueDictionary();
                }
                if (page <0)
                    routeDictionary.Add(parameterName, templateSymbol);
                else
                    routeDictionary.Add(parameterName, page);
                pageUrl = UrlHelper.GenerateUrl(
                    routeName,
                    actionName,
                    controllerName,
                    protocol,
                    hostname,
                    fragment,
                    routeDictionary,
                    htmlHelper.RouteCollection,
                    htmlHelper.ViewContext.RequestContext,
                    true);

            }

            htmlAttributes["id"] = buttonId;
            string res;
            BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "cursor", "pointer");
            switch (pageButtonStyle)
            {
                
                case PageButtonStyle.Button:
                    htmlAttributes["type"] = "button";
                    htmlAttributes["value"] = htmlHelper.Encode(textOrUrl);
                    return 
                       string.Format(buttonSchema,
                           BasicHtmlHelper.GetAttributesString(htmlAttributes),
                           fieldName,
                           pageString(page),
                           buttonId,
                           clientUrl(page, pageUrl),
                           targetIdName,
                           validationType);
                case PageButtonStyle.Link:
                    htmlAttributes["href"] = "javascript:void(0);";
                    
                    if (htmlAttributes.ContainsKey("disabled") && htmlAttributes["disabled"].ToString() == "disabled" ||
                        htmlAttributes.ContainsKey("data-selected-page") && htmlAttributes["data-selected-page"].ToString() == "selected")
                    {
                            string prevstyle = BasicHtmlHelper.SetStyle(htmlAttributes, "color", "gray"); 
                            res=
                                string.Format(linkSchemaDisabled,
                                    BasicHtmlHelper.GetAttributesString(htmlAttributes),
                                    fieldName,
                                    page,
                                    buttonId,
                                    encoded ? htmlHelper.Encode(textOrUrl) : textOrUrl,
                                    pageUrl,
                                    targetIdName,
                                    validationType);
                            BasicHtmlHelper.SetAttribute(htmlAttributes, "style", prevstyle);
                    }
                    else
                    {
                        if (pageUrl.Length != 0 && targetIdName.Length==0 && page >= 0)
                        {
                             htmlAttributes["href"] =pageUrl;
                             res = string.Format(linkSchemaGet, BasicHtmlHelper.GetAttributesString(htmlAttributes),
                                 encoded ? htmlHelper.Encode(textOrUrl) : textOrUrl);
                        }
                        else
                        {
                            res =
                                string.Format(linkSchema,
                                    BasicHtmlHelper.GetAttributesString(htmlAttributes),
                                    fieldName,
                                    pageString(page),
                                    buttonId,
                                     encoded ? htmlHelper.Encode(textOrUrl) : textOrUrl,
                                     clientUrl(page, pageUrl),
                                     targetIdName,
                                     validationType);
                        }
                    }
                    return res;
                default:
                    htmlAttributes["src"] = textOrUrl;
                    
                    if (htmlAttributes.ContainsKey("disabled") && htmlAttributes["disabled"].ToString() == "disabled")
                    {
                        string prevstyle = BasicHtmlHelper.SetStyle(htmlAttributes, "color", "gray"); 
                        res=
                            string.Format(imgSchemaDisabled,
                               BasicHtmlHelper.GetAttributesString(htmlAttributes),
                               fieldName,
                               page,
                               buttonId,
                               pageUrl,
                               targetIdName,
                               validationType);
                        BasicHtmlHelper.SetAttribute(htmlAttributes, "style", prevstyle);
                    }
                    else
                    {
                        
                        res =
                            string.Format(imgSchema,
                               BasicHtmlHelper.GetAttributesString(htmlAttributes),
                               fieldName,
                               pageString(page),
                               buttonId,
                               clientUrl(page, pageUrl),
                               targetIdName,
                               validationType);
                    }
                    return res;
            }
        }
        public MvcHtmlString GoToText(IDictionary<string, object> htmlAttributes, ContentAlign contentAlign=ContentAlign.Right)
        {
            uint val = (uint)currPage;
            return htmlHelper.TypedTextBox(goTextName, val, htmlAttributes, contentAlign: contentAlign);
        }
        public MvcHtmlString GoToText(object htmlAttributes = null, ContentAlign contentAlign = ContentAlign.Right)
        {
            uint val = (uint)currPage;
            return htmlHelper.TypedTextBox(goTextName, val, htmlAttributes, contentAlign: contentAlign);
        }
        public MvcHtmlString PageButton(string textOrUrl, PageButtonType pageButtonType,
            PageButtonStyle pageButtonStyle = PageButtonStyle.Link, IDictionary<string, object> htmlAttributes = null, string disabledTextOrUrl=null)
        {
            if (string.IsNullOrWhiteSpace(textOrUrl)) throw (new ArgumentNullException("textOrUrl"));
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            int page = -1 ;
            switch (pageButtonType)
            {
                case PageButtonType.First:
                    page = 1;
                    if (currPage <= 1)
                    {
                        htmlAttributes["disabled"] = "disabled";
                        if (disabledTextOrUrl != null) textOrUrl = disabledTextOrUrl;
                    }
                     break;
                case PageButtonType.Last:
                    if (totPages != null && totPages.HasValue && totPages.Value > currPage)
                    {
                        page = totPages.Value;
                        
                    }
                    else
                    {
                        htmlAttributes["disabled"] = "disabled";
                        if (disabledTextOrUrl != null) textOrUrl = disabledTextOrUrl;
                        page = currPage;
                    }
                    break;
                case PageButtonType.Next:
                    if (totPages != null && totPages.HasValue)
                    {
                        if (totPages> currPage)
                            page = currPage+1;
                        else
                        {
                            page=currPage;
                            htmlAttributes["disabled"] = "disabled";
                            if (disabledTextOrUrl != null) textOrUrl = disabledTextOrUrl;
                        }
                    }
                    else
                    {
                        
                        page = currPage+1;
                    }
                    break;
                case PageButtonType.Previous: 
                    page = 1;
                    if (currPage <= 1)
                    {
                        htmlAttributes["disabled"] = "disabled";
                        if (disabledTextOrUrl != null) textOrUrl = disabledTextOrUrl;
                    }
                    else
                    {
                        page = currPage - 1;
                    }
                    break;
                case PageButtonType.GoTo:
                    page = -1;
                    if (totPages.HasValue && totPages <=1)
                    {
                        htmlAttributes["disabled"] = "disabled";
                        if (disabledTextOrUrl != null) textOrUrl = disabledTextOrUrl;
                    }
                    break;
                default: break ;
            }
            return
                MvcHtmlString.Create(
                    RenderPageField() +
                    RenderPageButton(
                        getPageButtonClass(pageButtonType),
                        textOrUrl,
                        page,
                        pageButtonStyle,
                        htmlAttributes));       
                        
        }
        public MvcHtmlString PageChoice(int pagesToShow, IDictionary<string, object> htmlAttributes = null, Func<int, string> pageNames=null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();
            sb.Append(RenderPageField());
            int min = currPage - pagesToShow;
            if (min <= 1) min = 1;
            int max = currPage + pagesToShow;
            if (totPages != null && totPages.HasValue && totPages.Value < max)
                max = totPages.Value;
            for (int page = min; page <= max; page++)
            {
                if (page == currPage) htmlAttributes["data-selected-page"] = "selected";
                else if (htmlAttributes.ContainsKey("data-selected-page")) htmlAttributes.Remove("data-selected-page");
                if (page != min) sb.Append("&nbsp;");
                if (pageNames == null)
                    sb.Append(
                        RenderPageButton(page.ToString(), page.ToString(), page, PageButtonStyle.Link, htmlAttributes, false));
                else
                    sb.Append(
                        RenderPageButton(page.ToString(), pageNames(page), page, PageButtonStyle.Link, htmlAttributes, false));
            }
            return MvcHtmlString.Create(sb.ToString());
        }
        
    }
}

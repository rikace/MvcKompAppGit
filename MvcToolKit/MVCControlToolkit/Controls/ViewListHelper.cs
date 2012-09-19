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
using MVCControlsToolkit.Controls.DataFilter;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    
    public static class ViewListHelper
    {
        private static string viewSelectionScriptUnobtrusiveClient =
            @"
            <script language='javascript' type='text/javascript'>
                var {0}_ViewList = null;
                $('.{0}').hide();
                $(document).ready(function()
                {{" +

                    "setTimeout(\"$('.{0}').show(); {0}_ViewList = new ViewList_Client('{0}', '{1}', '{3}', '{4}'); $('.{0}').show(); {0}_ViewList.Select('{2}');\", 0);" +
            @"
                }});
            </script>   
            ";
        private static string viewSelectionScriptNoClient =
            @"
            <script language='javascript' type='text/javascript'>
                var {0}_ViewList = null;
                $('.{0}').hide();
                $(document).ready(function()
                {{" +

                    "setTimeout(\"$('.{0}').show(); {0}_ViewList = new ViewList_Client('{0}', '{1}', '{3}', '{4}'); {0}_ViewList.Select('{2}');\", 0);" +
            @"
                }});
            </script>   
            ";
        private static string viewSelectionScriptStandardClient =
             @"
            <script language='javascript' type='text/javascript'>
                var {0}_ViewList = null;
                $('.{0}').hide();
                $(document).ready(function()
                {{" +

                    "setTimeout(\"$('.{0}').show(); {0}_ViewList = new ViewList_Client('{0}', '{1}', '{3}', '{4}'); {0}_ViewList.Select('{2}');\", 0);" +
            @"
                }});
            </script>   
            ";
        private static string buttonSchema =
           @"
            <input {0} />
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                    {1}_ViewList.Select('{2}');
                }});
               
            </script>
            ";
        private static string linkSchema =
            @"
            <a {0}>{4}</a>
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                    {1}_ViewList.Select('{2}');
                }});
               
            </script>
            ";
        private static string imgSchema =
            @"
            <img {0} />
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                     {1}_ViewList.Select('{2}');
                }});
               
            </script>
            ";

        private static string viewsOnOfScriptScriptUnobtrusiveClient =
            @"
            <script language='javascript' type='text/javascript'>
                var {0}_ViewsOnOff = null;
                $('.{0}').hide();
                $(document).ready(function()
                {{" +
                    "setTimeout(\"$('.{0}').show(); ViewsOnOff_Client_Initialize('{0}', {1}, '{2}');  \", 0); " +
           @" 
                }});
            </script>   
            ";
        private static string viewsOnOfScriptScriptNoClient =
            @"
            <script language='javascript' type='text/javascript'>
                var {0}_ViewsOnOff = null;
                $('.{0}').hide();
                $(document).ready(function()
                {{" +
                    "setTimeout(\"$('.{0}').show(); ViewsOnOff_Client_Initialize('{0}', {1}, '{2}');  \", 0); " +
           @" 
                }});
            </script>   
            ";
        private static string viewsOnOfScriptStandardClient =
           @"
            <script language='javascript' type='text/javascript'>
                var {0}_ViewsOnOff = null;
                $('.{0}').hide();
                $(document).ready(function()
                {{" +
                    "setTimeout(\"$('.{0}').show(); ViewsOnOff_Client_Initialize('{0}', {1}, '{2}'); \", 0);  " +
           @" 
                }});
            </script>   
            ";

        public static MvcHtmlString ViewsOnOff<M>(this HtmlHelper<M> htmlHelper, string groupName, bool initialOn)
        {
            if (groupName == null) throw (new ArgumentNullException("groupName"));
            string viewsOnOffScript = null;
            string partialName = groupName + "_selection";
            string fullName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialName);
            string fullId = BasicHtmlHelper.IdFromName(fullName);
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: viewsOnOffScript = viewsOnOfScriptStandardClient; break;
                case ValidationType.UnobtrusiveClient: viewsOnOffScript = viewsOnOfScriptScriptUnobtrusiveClient; break;
                default: viewsOnOffScript = viewsOnOfScriptScriptNoClient; break;
            }
            ValueProviderResult vr = htmlHelper.ViewContext.Controller.ValueProvider.GetValue(fullName);
            if (vr != null)
            {
                initialOn = (bool)(vr.ConvertTo(typeof(bool)));
                  
            }
            return MvcHtmlString.Create(
                htmlHelper.Hidden(partialName, initialOn) +
                string.Format(viewsOnOffScript, groupName, initialOn ? "true":"false", fullId)
                );
        }

        public static MvcHtmlString ViewListFor<M>(
            this HtmlHelper<M> htmlHelper,
            Expression<Func<M, string>> expression,
            string groupName,
            string cssSelected)
        {
            if (expression == null) throw (new ArgumentNullException("expression"));
            if (groupName == null) throw (new ArgumentNullException("groupName"));

            string viewSelectionScript = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: viewSelectionScript = viewSelectionScriptStandardClient; break;
                case ValidationType.UnobtrusiveClient: viewSelectionScript = viewSelectionScriptUnobtrusiveClient; break;
                default: viewSelectionScript = viewSelectionScriptNoClient; break;
            }

            string partialName = ExpressionHelper.GetExpressionText(expression);
            string fullId = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialName));

            string selection = string.Empty;

            try
            {
                selection = expression.Compile().Invoke(htmlHelper.ViewData.Model) as string ;
            }
            catch
            {

            }
            string prefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = string.Empty;
            else
                prefix = BasicHtmlHelper.IdFromName(prefix) + "_";
            if (string.IsNullOrWhiteSpace(cssSelected)) cssSelected = string.Empty;
            return
                MvcHtmlString.Create(
                    htmlHelper.Hidden(partialName, selection) +
                    string.Format(viewSelectionScript, groupName, fullId, selection, cssSelected, prefix));

        }
        public static MvcHtmlString ViewList<M>(
            this HtmlHelper<M> htmlHelper,
            string groupName,
            string cssSelected,
            string selection=null)
        {
            if (groupName == null) throw (new ArgumentNullException("groupName"));
            string partialName = groupName+"_selection";
            string fullId = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialName));
            
            string viewSelectionScript = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: viewSelectionScript = viewSelectionScriptStandardClient; break;
                case ValidationType.UnobtrusiveClient: viewSelectionScript = viewSelectionScriptUnobtrusiveClient; break;
                default: viewSelectionScript = viewSelectionScriptNoClient; break;
            }

            if (string.IsNullOrWhiteSpace(selection)) 
                selection = string.Empty;
            string prefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = string.Empty;
            else
                prefix = BasicHtmlHelper.IdFromName(prefix) + "_";
            ValueProviderResult vr =
                htmlHelper.ViewContext.Controller.ValueProvider.GetValue(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(partialName));
            if (vr != null)
            {
                string oldres = vr.ConvertTo(typeof(string)) as string;
                if (!string.IsNullOrWhiteSpace(oldres)) selection = oldres;  
            }
            if (string.IsNullOrWhiteSpace(cssSelected)) cssSelected = string.Empty;
            return
                MvcHtmlString.Create(
                    htmlHelper.Hidden(partialName, selection) +
                    string.Format(viewSelectionScript, groupName, fullId, selection, cssSelected, prefix));

        }
/*
        public static MvcHtmlString OnOffViewFor<M>(
            this HtmlHelper<M> htmlHelper,
            Expression<Func<M, bool>> expression,
            string elementId
            )
        {
            if (elementId == null) throw (new ArgumentNullException("elementId"));
            if (expression == null) throw (new ArgumentNullException("expression"));

            string partialName = ExpressionHelper.GetExpressionText(expression);
            string fullId = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(partialName);

            bool selection = false;

            try
            {
                selection = (bool)(expression.Compile().Invoke(htmlHelper.ViewData.Model));
            }
            catch
            {

            }

            return
                MvcHtmlString.Create(
                    htmlHelper.Hidden(partialName, selection) +
                    string.Format(onOffViewSelectionScript, elementId, fullId, selection));

        }
        */
   /*     public static MvcHtmlString SelectButton<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            string textOrUrl,
            Expression<Func<VM, T>> target,
            string groupName,
            string name,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (textOrUrl == null) throw new ArgumentNullException("textOrUrl");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(groupName)) throw new ArgumentNullException("groupName");
            string targetName = BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(target)));
            return
                SelectionButton<VM>(
                    htmlHelper,
                    textOrUrl,
                    targetName,
                    groupName,
                    name,
                    manipulationButtonStyle,
                    htmlAttributes);
        }*/

        public static MvcHtmlString SelectionButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            string textOrUrl,
            string targetName,
            string groupName,
            string name ,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");
            if (textOrUrl == null) throw new ArgumentNullException("textOrUrl");
            if (targetName == null) throw new ArgumentNullException("targetName");
            if (string.IsNullOrWhiteSpace(groupName)) throw new ArgumentNullException("groupName");

            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();

            string buttonId = BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, name));
            htmlAttributes["id"] = buttonId;
            BasicHtmlHelper.AddClass(htmlAttributes, groupName + "_button");
            BasicHtmlHelper.AddClass(htmlAttributes, htmlHelper.PrefixedId(targetName) + "_button");
            switch (manipulationButtonStyle)
            {
                case ManipulationButtonStyle.Button:
                    htmlAttributes["type"] = "button";
                    htmlAttributes["value"] = htmlHelper.Encode(textOrUrl);
                    return MvcHtmlString.Create(
                       string.Format(buttonSchema,
                           BasicHtmlHelper.GetAttributesString(htmlAttributes),
                           groupName,
                           targetName,
                           buttonId));
                case ManipulationButtonStyle.Link:
                    htmlAttributes["href"] = "javascript:void(0);";
                    return MvcHtmlString.Create(
                    string.Format(linkSchema,
                        BasicHtmlHelper.GetAttributesString(htmlAttributes),
                        groupName,
                        targetName,
                        buttonId,
                        htmlHelper.Encode(textOrUrl)));
                default:
                    htmlAttributes["src"] = textOrUrl;
                    BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "cursor", "pointer");
                    return MvcHtmlString.Create(
                       string.Format(imgSchema,
                           BasicHtmlHelper.GetAttributesString(htmlAttributes),
                           groupName,
                           targetName,
                           buttonId));

            }
        }
    }
}

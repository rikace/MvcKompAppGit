
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


namespace MVCControlsToolkit.Controls
{
    public enum ManipulationButtonType { Remove, Show, Hide, ResetGrid, Custom };
    public enum ManipulationButtonStyle{Button, Link, Image};

   
    public static class ButtonHelpers
    {
       
        private static string buttonSchema =
           @"
            <input {0} />
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                    ManipulationButton_Click({1}, '{2}');
                }});
               
            </script>
            ";
        private static string linkSchema =
            @"
            <a {0}>{4}</a>
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                    ManipulationButton_Click({1}, '{2}');
                }});
               
            </script>
            ";
        private static string imgSchema =
            @"
            <img {0} />
            <script language='javascript' type='text/javascript'>
                $('#{3}').click(function() 
                {{
                     ManipulationButton_Click({1}, '{2}');
                }});
               
            </script>
            ";
        static private string getManipulationButtonClass(ManipulationButtonType manipulationButtonType)
        {
            switch(manipulationButtonType)
            {
                case ManipulationButtonType.Remove: return "ManipulationButtonRemove";
                case ManipulationButtonType.Hide: return "ManipulationButtonHide";
                case ManipulationButtonType.Show: return "ManipulationButtonShow";
                case ManipulationButtonType.ResetGrid: return "ManipulationButtonResetGrid";
                default: return "ManipulationButtonCustom";
            }
        }
        static private string processTarget(ManipulationButtonType manipulationButtonType, string target)
        {
            target = target.Trim();
            if (manipulationButtonType == ManipulationButtonType.Custom && target.StartsWith("function"))
            {
                return target;
            }
            else return "'" + target + "'";
        }
        public static MvcHtmlString ManipulationButton<VM, T>(
            this HtmlHelper<VM> htmlHelper,
            ManipulationButtonType manipulationButtonType,
            string textOrUrl,
            Expression<Func<VM, T>> target,
            string name = null,
            ManipulationButtonStyle manipulationButtonStyle = ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes = null)
        {
            if (target == null) throw new ArgumentNullException("target");
            string targetName=BasicHtmlHelper.IdFromName(htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(
                ExpressionHelper.GetExpressionText(target)));
            return
                ManipulationButton<VM>(
                    htmlHelper,
                    manipulationButtonType,
                    textOrUrl,
                    targetName,
                    name,
                    manipulationButtonStyle,
                    htmlAttributes);
        }

        public static MvcHtmlString ManipulationButton<VM>(
            this HtmlHelper<VM> htmlHelper,
            ManipulationButtonType manipulationButtonType,
            string textOrUrl,
            string targetName,
            string name=null,
            ManipulationButtonStyle manipulationButtonStyle= ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes=null)
        {
            if (string.IsNullOrWhiteSpace(name)) name=getManipulationButtonClass(manipulationButtonType);
            if (textOrUrl == null) throw new ArgumentNullException("textOrUrl");
            if (targetName == null) throw new ArgumentNullException("targetName");

            if (htmlAttributes==null) htmlAttributes= new Dictionary<string, object>();
            
            string buttonId=BasicHtmlHelper.IdFromName(
                BasicHtmlHelper.AddField(htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, name));
            htmlAttributes["id"] = buttonId;
            switch(manipulationButtonStyle)
            {
                case ManipulationButtonStyle.Button:
                     htmlAttributes["type"] = "button";
                     htmlAttributes["value"] = htmlHelper.Encode(textOrUrl);
                     return MvcHtmlString.Create(
                        string.Format(buttonSchema,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes),
                            processTarget(manipulationButtonType,targetName),
                            getManipulationButtonClass(manipulationButtonType),
                            buttonId));
                case ManipulationButtonStyle.Link:
                    htmlAttributes["href"] = "javascript:void(0);";
                    return MvcHtmlString.Create(
                    string.Format(linkSchema,
                        BasicHtmlHelper.GetAttributesString(htmlAttributes),
                        processTarget(manipulationButtonType, targetName),
                        getManipulationButtonClass(manipulationButtonType),
                        buttonId,
                        htmlHelper.Encode(textOrUrl)));
                default:
                    htmlAttributes["src"] = textOrUrl;
                    BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "cursor", "pointer");
                     return MvcHtmlString.Create(
                        string.Format(imgSchema,
                            BasicHtmlHelper.GetAttributesString(htmlAttributes),
                            processTarget(manipulationButtonType, targetName),
                            getManipulationButtonClass(manipulationButtonType),
                            buttonId));

            }
        }/*
        public static MvcHtmlString SortButtonFor<VM, TItem, TField>
            (this HtmlHelper<VM> htmlHelper,

            RenderInfo<List<Tracker<TItem>>> renderInfo,
            Expression<Func<TItem, TField>> field,
            Expression<Func<TItem, int>> page,
            string textOrUrl,
            string textOrUrlAscending,
            string cssAscending,
            string textOrUrlDescending,
            string cssDescending,
            ManipulationButtonStyle manipulationButtonStyle= ManipulationButtonStyle.Button,
            IDictionary<string, object> htmlAttributes=null)
        {

        }*/
    }
}

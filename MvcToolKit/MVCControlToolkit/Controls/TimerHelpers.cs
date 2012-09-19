using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using MVCControlsToolkit.Core;

namespace MVCControlsToolkit.Controls
{
    
    public static class TimerHelpers
    {
        private static string getTimerScript = @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function () {{
                    setInterval('MvcControlsToolkit_AjaxLink(\'{0}\', \'{1}\', {3}, {4});', {2});
                }});
                
            </script>
        ";
        private static string submitSymbol = "QS_23459_86{0}45_ZA";
        private static string submitTimerScript = @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function () {{
                    setInterval('MvcControlsToolkit_AjaxSubmit(\'{0}\', \'{1}\');', {2});
                }});    
            </script>
            <span id = '{1}' style='display:none' ></span>
        ";
        private static string saveAndSubmitTimerScript = @"
            <script language='javascript' type='text/javascript'>
                $(document).ready(function () {{
                    setInterval('{0}.saveAndSubmit();', {1});
                }});
                
            </script>
        ";
        public static MvcHtmlString SubmitTimer<VM>(
            this HtmlHelper<VM> htmlHelper, TimeSpan timeInterval, string clientViewModel=null)
        {
            if (clientViewModel != null)
            {
                return MvcHtmlString.Create(
                    string.Format(saveAndSubmitTimerScript, clientViewModel, timeInterval.TotalMilliseconds));
            
            }
            string validationType = null;
            switch (MvcEnvironment.Validation(htmlHelper))
            {
                case ValidationType.StandardClient: validationType = "StandardClient"; break;
                case ValidationType.UnobtrusiveClient: validationType = "UnobtrusiveClient"; break;
                default: validationType = "Server"; break;
            }
            string targetIdName = BasicHtmlHelper.GetUniqueSymbol(htmlHelper, submitSymbol);
            return MvcHtmlString.Create(
                string.Format(submitTimerScript, validationType, targetIdName, timeInterval.TotalMilliseconds)
            );
        }
        public static MvcHtmlString GetTimer<VM>(
            this HtmlHelper<VM> htmlHelper, TimeSpan timeInterval,
           string actionName,
           string targetIdName,
           string controllerName = null,
           object parameters = null,
           string routeName = null,
           string protocol = null,
           string hostname = null,
           string fragment = null,
            InsertionMode mode = InsertionMode.Replace,
            string afterSuccess=null
           )
        {

            if (string.IsNullOrWhiteSpace(targetIdName)) throw new ArgumentNullException("targetIdName");
            
            System.Web.Routing.RouteValueDictionary routeDictionary=null;
            if (parameters != null)
            {
                routeDictionary=new System.Web.Routing.RouteValueDictionary(parameters);
            }
            else
            {
                routeDictionary=new System.Web.Routing.RouteValueDictionary();
            }
            string ajaxUrl=UrlHelper.GenerateUrl(
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
            string modeS;
            switch (mode)
            {
                case InsertionMode.Replace: modeS = "0"; break;
                case InsertionMode.InsertBefore: modeS = "-1"; break;
                case InsertionMode.InsertAfter: modeS = "1"; break;
                default: modeS = "1"; break;
            }
            return MvcHtmlString.Create(
                string.Format(getTimerScript, ajaxUrl, targetIdName, timeInterval.TotalMilliseconds, modeS, afterSuccess == null ? "null" : afterSuccess)
            );
        }
        
    }
}

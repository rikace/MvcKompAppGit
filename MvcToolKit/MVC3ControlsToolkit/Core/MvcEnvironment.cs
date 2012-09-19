using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCControlsToolkit.Core
{
    public enum ValidationType {StandardClient, UnobtrusiveClient, Server}
    public static class MvcEnvironment
    {
        public static IDictionary<string, object> GetUnobtrusiveValidation(HtmlHelper htmlHelper, string name, ModelMetadata metaData = null)
        {

            return metaData == null ? htmlHelper.GetUnobtrusiveValidationAttributes(name) : htmlHelper.GetUnobtrusiveValidationAttributes(name, metaData);
        }
        public static ValidationType Validation(HtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.ClientValidationEnabled)
            {
                if (htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                    return ValidationType.UnobtrusiveClient;
                else
                    return ValidationType.StandardClient;
            }
            else
            {
                return ValidationType.Server;
            }
        }

        public static ValidationType Validation(ViewContext context)
        {
            if (context.ClientValidationEnabled)
            {
                if (context.UnobtrusiveJavaScriptEnabled) return ValidationType.UnobtrusiveClient;
                else return ValidationType.StandardClient;
            }
            else
            {
                return ValidationType.Server;
            }
        }
        
        public static bool UnobtrusiveAjaxOn(HtmlHelper htmlHelper)
        {
            return htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled;
        }
    }
}

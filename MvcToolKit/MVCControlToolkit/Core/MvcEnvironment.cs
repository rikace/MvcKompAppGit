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
            return new Dictionary<string, object>(); ;
        }
        public static ValidationType Validation(HtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.ClientValidationEnabled)
            {
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
                return ValidationType.StandardClient;
            }
            else
            {
                return ValidationType.Server;
            }
        }
        public static bool UnobtrusiveAjaxOn(HtmlHelper htmlHelper)
        {
            return false;
        }
    }
}

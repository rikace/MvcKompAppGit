using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MVCControlsToolkit.Core;
using System.Globalization;

namespace MVCControlsToolkit.Controls.Validation
{
    public static class ValidationHelpers
    {
        private static string scriptJQuery = @"
        <script type='text/javascript' src='{0}'></script>
        <script type='text/javascript'>
            
            $.validator.methods.number = function (value, element) {{
                if (value == '' || !isNaN(jQuery.global.parseFloat(value))) {{
                    return true;
                }}
                return false;
            }}
            $.validator.methods.date = function (value, element) {{
                if (value == '' || !isNaN(jQuery.global.parseDate(value))) {{
                    return true;
                }}
                return false;
            }}
           
            $(document).ready(function () {{
                jQuery.global.culture = jQuery.global.cultures['{1}'];
                jQuery.global.preferCulture(jQuery.global.name);
                var culture=jQuery.global.culture;
                culture.calendar.patterns['G']=culture.calendar.patterns.d+' '+culture.calendar.patterns.T;
            }});
            
        </script>
        ";
        private static string script = @"
        <script type='text/javascript' src='{0}'></script>
        <script type='text/javascript'>
            jQuery.global = Globalize;
            $.validator.methods.number = function (value, element) {{
                if (value == '' || !isNaN(jQuery.global.parseFloat(value))) {{
                    return true;
                }}
                return false;
            }}
            $.validator.methods.date = function (value, element) {{
                if (value == '' || !isNaN(jQuery.global.parseDate(value))) {{
                    return true;
                }}
                return false;
            }}
           
            $(document).ready(function () {{
                jQuery.global.culture('{1}');
                var culture=Globalize.culture();
                culture.calendar.patterns['G']=culture.calendar.patterns.d+' '+culture.calendar.patterns.T;
            }});
            
        </script>
        ";
        
        private static string scriptDataPicker = @"
            <script type='text/javascript' src='{0}'></script>          
            
        ";

        
        public static MvcHtmlString jQueryGlobalizationScript(this HtmlHelper htmlHelper, string globalizationFolder = "~/Scripts/globinfo/", string localizationFolder="~/Scripts/localization/")
        {
            string cultureName=System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            
            string culturePath = string.Format("{0}jquery.glob.{1}.js", globalizationFolder, cultureName);
            if (culturePath[0] == '~') culturePath = UrlHelper.GenerateContentUrl(culturePath, htmlHelper.ViewContext.HttpContext);

            return MvcHtmlString.Create(string.Format(scriptJQuery, culturePath, cultureName));

        }
        public static MvcHtmlString GlobalizationScript(this HtmlHelper htmlHelper, string globalizationFolder = "~/Scripts/cultures/", string localizationFolder = "~/Scripts/localization/")
        {
            string cultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;

            string culturePath = string.Format("{0}globalize.culture.{1}.js", globalizationFolder, cultureName);
            if (culturePath[0] == '~') culturePath = UrlHelper.GenerateContentUrl(culturePath, htmlHelper.ViewContext.HttpContext);

            return MvcHtmlString.Create(string.Format(script, culturePath, cultureName));

        }
        public static MvcHtmlString JQueryDatePickerGlobalizationScript(this HtmlHelper htmlHelper, string cultureName=null, string globalizationFolder = "http://jquery-ui.googlecode.com/svn/trunk/ui/i18n/")
        {
            if (cultureName==null)
                cultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name.Substring(0, 2);
            string culturePath = string.Format("{0}jquery.ui.datepicker-{1}.js", globalizationFolder, cultureName);
            if (culturePath[0] == '~') culturePath = UrlHelper.GenerateContentUrl(culturePath, htmlHelper.ViewContext.HttpContext);


            return MvcHtmlString.Create(string.Format(scriptDataPicker, culturePath));

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.WebPages;

namespace MVCControlsToolkit.Controls
{
    public static class _S
    {
        public static object H<T>(Func<HtmlHelper<T>, HelperResult> template ){
            return template;
        }
        public static object L<T>(Func<HtmlHelper<T>, string> template)
        {
            return template;
        }
        
        public static HtmlHelper<T> HH<T>(HtmlHelper h)
        {
            return h as HtmlHelper<T>;
        }
        
    }
}

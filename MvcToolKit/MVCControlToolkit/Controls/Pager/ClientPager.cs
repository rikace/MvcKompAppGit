using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Globalization;
using MVCControlsToolkit.Core;
using MVCControlsToolkit.Controls.Bindings;

namespace MVCControlsToolkit.Controls
{
    public class ClientPager<VM>
    {
        private HtmlHelper<VM> htmlHelper;
        private int currPage;
        private string fieldName;
        private string fieldId;
        private bool pageFieldRendered;
        private int? totPages;
        private string pagePrefix;
        private string pagePostfix;
        private string goTextName;
        private string totPagesFieldName;
        private bool causeSubmit;
        private string validationType;
        public ClientPager(HtmlHelper<VM> htmlHelper, string fieldName, int currPage, string totPagesFieldName, int? totPages, string pagePrefix, string pagePostfix, bool causeSubmit)
        {
            this.htmlHelper = htmlHelper;
            this.currPage = currPage;
            this.fieldName=fieldName;
            this.fieldId = BasicHtmlHelper.IdFromName(fieldName);
            this.totPages = totPages;
            this.totPagesFieldName = totPagesFieldName;
            this.pageFieldRendered = false;
            this.pagePrefix = pagePrefix;
            this.pagePostfix = pagePostfix;
            goTextName=fieldId+"_goto" ;
            this.causeSubmit = causeSubmit;
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
            IBindingsBuilder<VM> bindings = htmlHelper.ClientBindings();
            string modelName =bindings == null ? "" : bindings.ModelName;
            
            return
                     string.Format(mainScript, 
                     currPage, 
                     fieldId, 
                     fieldName,
                     totPages.HasValue ? 
                         totPages.Value.ToString(CultureInfo.InvariantCulture) :
                         "",
                     pagePrefix == null ? "" : pagePrefix, 
                     pagePostfix==null ? "" : pagePostfix,
                     modelName,
                     totPagesFieldName == null ? "" : totPagesFieldName,
                     causeSubmit ? "data-cause-submit='true'" : "",
                     validationType
                     );
           
        }
        private static string mainScript =
        @"
        <input type='hidden' value='{0}' id='{1}'  data-client-model='{6}' data-total-pages = '{3}' data-total-pages-name='{7}' data-page-prefix='{4}' data-page-postfix='{5}' data-element-type='ClientPager' {8} data-validation-type='{9}' name='{2}'/>
         <script type='text/javascript'>
            MvcControlsToolkit_InitJsonPager('{1}');
         </script>
        ";
        private static string buttonSchema =
            @"
            <input {0} />
            ";
        private static string linkSchema =
            @"
            <a {0}>{1}</a>
            </script>
            ";
        private static string imgSchema =
            @"
            <img {0} />
            ";
        private string getPageButtonClass(PageButtonType? pageButtonType)
        {
            if (!pageButtonType.HasValue) return "PageButtonPage";
            switch (pageButtonType.Value)
            {
                case PageButtonType.First: return "PageButtonFirst";
                case PageButtonType.Last: return "PageButtonLast";
                case PageButtonType.Next: return "PageButtonNext";
                case PageButtonType.Previous: return "PageButtonPrevious";
                case PageButtonType.GoTo: return "PageButtonGoTo";
                default: return string.Empty; ;
            }
        }
        protected string RenderPageButton(string textOrUrl, string disabledTexOrUrl, int index,
            PageButtonStyle pageButtonStyle, PageButtonType? pageButtonType, IDictionary<string, object> htmlAttributes = null, bool encoded = true)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            string buttonType = getPageButtonClass(pageButtonType);
            string buttonId = fieldId + "_" + buttonType;
            if (!pageButtonType.HasValue)
            {
                if (index < 0) buttonId = buttonId + "_" + (-index).ToString(CultureInfo.InvariantCulture);
                else buttonId = buttonId + index.ToString(CultureInfo.InvariantCulture);
                htmlAttributes["data-pager-index"] = index.ToString(CultureInfo.InvariantCulture);
            }
            htmlAttributes["id"] = buttonId;
            htmlAttributes["data-pager-button"] = buttonType;
            BasicHtmlHelper.AddClass(htmlAttributes, fieldId + "_class");
            BasicHtmlHelper.SetDefaultStyle(htmlAttributes, "cursor", "pointer");
            switch (pageButtonStyle)
            {
                case PageButtonStyle.Button:
                    htmlAttributes["type"] = "button";
                    htmlAttributes["value"] = htmlHelper.Encode(textOrUrl);
                    return string.Format(buttonSchema, BasicHtmlHelper.GetAttributesString(htmlAttributes));
                case PageButtonStyle.Image:
                    htmlAttributes["src"] = textOrUrl;
                    if (disabledTexOrUrl != null) 
                    {
                        htmlAttributes["data-disabled-src"] = disabledTexOrUrl;
                        htmlAttributes["data-enabled-src"] = textOrUrl;
                    }
                    return string.Format(imgSchema, BasicHtmlHelper.GetAttributesString(htmlAttributes));
                default:
                    htmlAttributes["href"] = "javascript:void(0);";
                    return string.Format(linkSchema, BasicHtmlHelper.GetAttributesString(htmlAttributes), encoded ? htmlHelper.Encode(textOrUrl): textOrUrl);

            }
        }
        public MvcHtmlString GoToText(IDictionary<string, object> htmlAttributes, ContentAlign contentAlign = ContentAlign.Right)
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
            PageButtonStyle pageButtonStyle = PageButtonStyle.Link, IDictionary<string, object> htmlAttributes = null, string disabledUrl=null)
        {
            if (string.IsNullOrWhiteSpace(textOrUrl)) throw (new ArgumentNullException("textOrUrl"));
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            return
                MvcHtmlString.Create(
                    RenderPageField() +
                    RenderPageButton(
                        textOrUrl,
                        disabledUrl,
                        0,
                        pageButtonStyle,
                        pageButtonType,
                        htmlAttributes));                      
        }
        public MvcHtmlString PageChoice(int pagesToShow, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();
            sb.Append(RenderPageField());
            int min =  - pagesToShow;
            int max = pagesToShow;
            
            for (int page = min; page <= max; page++)
            {
                sb.Append("<span> ");
                sb.Append(
                        RenderPageButton("", null, page, PageButtonStyle.Link, null, htmlAttributes, false));
                sb.Append(" </span>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}

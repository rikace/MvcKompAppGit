using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace MvcKomp3
{
    public static class AjaxExtensions
    {
        public static MvcHtmlString Textbox(this AjaxHelper ajaxHelper, string name, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var tag = new TagBuilder("input");
            tag.MergeAttribute("name", name);
            tag.MergeAttribute("type", "text");
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            tag.MergeAttributes((ajaxOptions ?? new AjaxOptions()).ToUnobtrusiveHtmlAttributes());
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }
    }
}
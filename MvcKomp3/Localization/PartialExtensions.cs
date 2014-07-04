using System;
using System.Web.Mvc;

namespace BookSamples.Components.Localization
{
    public static class PartialExtensions
    {
        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, String partialViewName, Object model, ViewDataDictionary viewData, Boolean localizable = false)
        {
            // Attempt to get a localized view name
            var viewName = partialViewName;
            if (localizable)
                viewName = GetLocalizedViewName(htmlHelper, viewName);

            // Call the system Partial method
            return System.Web.Mvc.Html.PartialExtensions.Partial(htmlHelper, viewName, model, viewData);
        }

        public static MvcHtmlString Partial(this HtmlHelper htmlHelper, String partialViewName, Boolean localizable=false)
        {
            // Attempt to get a localized view name
            var viewName = partialViewName;
            if (localizable)
                viewName = GetLocalizedViewName(htmlHelper, viewName, isView:true);

            // Call the system Partial method
            return htmlHelper.Partial(viewName, null, htmlHelper.ViewData);
        }

        public static String GetLocalizedViewName(HtmlHelper htmlHelper, String partialViewName, Boolean isView=false)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            return UrlExtensions.GetLocalizedUrl(urlHelper, partialViewName, isView);
        }
    }
}

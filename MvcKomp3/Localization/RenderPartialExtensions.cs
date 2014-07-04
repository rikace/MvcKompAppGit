using System;
using System.Web.Mvc;

namespace BookSamples.Components.Localization
{
    public static class RenderPartialExtensions
    {
        public static void RenderPartial(this HtmlHelper htmlHelper, String partialViewName, Object model, ViewDataDictionary viewData, Boolean localizable = false)
        {
            // Attempt to get a localized view name
            var viewName = partialViewName;
            if (localizable)
                viewName = PartialExtensions.GetLocalizedViewName(htmlHelper, viewName);

            // Call the system Partial method
            System.Web.Mvc.Html.RenderPartialExtensions.RenderPartial(htmlHelper, viewName, model, viewData);
        }

        public static void RenderPartial(this HtmlHelper htmlHelper, String partialViewName, Boolean localizable = false)
        {
            // Attempt to get a localized view name
            var viewName = partialViewName;
            if (localizable)
                viewName = PartialExtensions.GetLocalizedViewName(htmlHelper, viewName);

            // Call the system Partial method
            htmlHelper.Partial(viewName, null, htmlHelper.ViewData);
        }
    }
}

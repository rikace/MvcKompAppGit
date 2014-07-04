using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using BookSamples.Components.Localization;

namespace BookSamples.Components.Html
{
    public static class ImageHelpers
    {
        public static MvcHtmlString ImageOrDefault(this HtmlHelper helper, String id, String imageUrl, Object htmlAttributes = null, String defaultImageUrl = "~/images/missing.png", String alternateText = "", Boolean localizable = false)
        {
            // Culture first
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var newImageUrl = imageUrl;
            if (localizable)
            {
                newImageUrl = UrlExtensions.GetLocalizedUrl(urlHelper, imageUrl);
                defaultImageUrl = UrlExtensions.GetLocalizedUrl(urlHelper, defaultImageUrl);
            }

            // Requested image or default image if not found
            return ImageOrDefaultInternal(helper, id, newImageUrl, htmlAttributes, defaultImageUrl, alternateText);
        }

        private static MvcHtmlString ImageOrDefaultInternal(this HtmlHelper helper, String id, String imageUrl, Object htmlAttributes = null, String defaultImageUrl = "~/images/missing.png", String alternateText = "")
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var imgSrc = urlHelper.Content(defaultImageUrl);

            // Check if referenced image really exists
            var imgVirtualPath = urlHelper.Content(imageUrl);
            var imgPhysicalPath = helper.ViewContext.HttpContext.Server.MapPath(imgVirtualPath);
            if (File.Exists(imgPhysicalPath))
                imgSrc = imgVirtualPath;

            var attribs = (IDictionary<String, Object>)new RouteValueDictionary(htmlAttributes);

            var img = new TagBuilder("img");
            img.MergeAttributes(attribs);
            img.MergeAttribute("id", id, true);
            img.MergeAttribute("src", imgSrc, true);
            img.MergeAttribute("alt", alternateText, true);

            return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
        }
    }
}
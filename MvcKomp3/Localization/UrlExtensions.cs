using System;
using System.IO;
using System.Threading;
using System.Web.Mvc;

namespace BookSamples.Components.Localization
{
    public static class UrlExtensions
    {
        public static String Content(this UrlHelper helper, String contentPath, Boolean localizable=false)
        {
            var url = contentPath;
            if (localizable)
            {
                url = GetLocalizedUrl(helper, url);
            }
            return helper.Content(url);
        }

        public static String GetLocalizedUrl(String resourceUrl)
        {
            var cultureExt = String.Format("{0}{1}",
                Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName, Path.GetExtension(resourceUrl));
            var url = Path.ChangeExtension(resourceUrl, cultureExt);
            return url;
        }

        public static String GetLocalizedUrl(UrlHelper helper, String resourceUrl, Boolean isView=false)
        {
            var url = GetLocalizedUrl(resourceUrl);
            return VirtualFileExists(helper, url, isView) ? url : resourceUrl;
        }

        public static Boolean VirtualFileExists(UrlHelper helper, String url, Boolean isView=false)
        {
            var fullVirtualPath = isView ? GetFullName(helper, url) : helper.Content(url);
            var physicalPath = helper.RequestContext.HttpContext.Server.MapPath(fullVirtualPath);
            return File.Exists(physicalPath);
        }

        private static String GetFullName(UrlHelper helper, String viewName)
        {
            var request = helper.RequestContext.HttpContext.Request;
            var index = request.FilePath.LastIndexOf('/');
            var controller = index < 0 ? "" : request.FilePath.Substring(0, index);

            var name = String.Format("~/Views{0}/{1}.cshtml", controller, viewName);
            return name;
        }
    }
}
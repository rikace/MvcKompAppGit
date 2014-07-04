using System;
using System.Web.Mvc;

namespace BookSamples.Components.Html
{
    public static class ImgHtmlHelpers 
    {
        public static MvcHtmlString InlineImage(this UrlHelper helper, String img, String  defaultImg="~/images/missing.png")
        {            
            // Data URI scheme
            const String dataUriFormat = "data:{0};base64,{1}";

            // Get bytes and Base64 encode
            var bytes = GetImageBytes(helper, img, defaultImg);
            var base64 = Convert.ToBase64String(bytes);

            // Return formatted
            return MvcHtmlString.Create(String.Format(dataUriFormat, "image/jpg", base64));
        }

        #region Helpers
        private static Byte[] GetImageBytes(UrlHelper helper, String img, String defaultImg)
        {
            var path = GetResolvedImageName(helper, img, defaultImg);
            var fs = System.IO.File.OpenRead(path);
            var data = new Byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();
            return data;
        }

        private static String GetResolvedImageName(UrlHelper helper, String img, String defaultImg)
        {
            var server = helper.RequestContext.HttpContext.Server;
            var path = server.MapPath(img);
            if (!System.IO.File.Exists(path))
            {
                path = server.MapPath(defaultImg);
            }
            return path;
        }
        #endregion
    }
}
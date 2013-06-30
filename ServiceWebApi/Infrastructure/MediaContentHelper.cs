using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ServiceWebApi.Infrastructure
{
    public static class MediaContentHelper
    {
        public static string GetContentTypeFromFileName(string fileName)
        {
            // Return the Content-Type value based on the file extension.
            switch (Path.GetExtension(fileName).ToLower())
            {
                case ".jpeg":
                    return "image/jpeg";
                case ".jpg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".tiff":
                    return "image/tiff";
                case ".bmp":
                    return "image/bmp";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
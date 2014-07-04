using System;
using System.Net;
using System.Web.Script.Serialization;

namespace BookSamples.Components.Extensions
{
    public static class WebClientExtensions
    {
         public static T DownloadJson<T>(this WebClient client, String url)
         {
             String data;
             try
             {
                 data = client.DownloadString(url);
             }
             catch
             {
                 data = null;
             }
             if (String.IsNullOrEmpty(data))
                 return default(T);

             var serializer = new JavaScriptSerializer();
             var obj = serializer.Deserialize<T>(data);
             return obj;
         }
    }
}